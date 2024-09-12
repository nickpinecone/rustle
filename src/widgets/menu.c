#include <jansson.h>
#include <memory.h>
#include <ncurses.h>
#include <stdio.h>

#include "keys.h"
#include "menu.h"

void menu_clean(struct menu *menu, int i) {
    char blank[menu->width - 1];
    memset(blank, ' ', menu->width - 2);
    blank[menu->width - 2] = '\0';
    mvwprintw(menu->win, i, 1, "%s", blank);
}

void menu_load_conf(struct menu *menu, struct conf *conf) {
    json_error_t error;

    menu->root = json_load_file(conf->riff_path, 0, &error);

    if (!menu->root) {
        fprintf(stderr, "config error: on line %d: %s\n", error.line,
                error.text);
        exit(1);
    }

    if (!json_is_array(menu->root)) {
        fprintf(stderr, "config error: root is not an array\n");
        json_decref(menu->root);
        exit(1);
    }

    size_t arr_size = json_array_size(menu->root);
    menu->items = malloc(sizeof(struct menu_item) * arr_size);
    menu->items_len = arr_size;

    for (int i = 0; i < arr_size; i++) {
        json_t *data, *name, *url;
        const char *name_text, *url_text;

        data = json_array_get(menu->root, i);
        if (!json_is_object(data)) {
            fprintf(stderr, "config error: station data #%d is not an object\n",
                    i + 1);
            json_decref(menu->root);
            exit(1);
        }

        name = json_object_get(data, "name");
        if (!json_is_string(name)) {
            fprintf(stderr, "config error: station #%d: name is not a string\n",
                    i + 1);
            json_decref(menu->root);
            exit(1);
        }
        name_text = json_string_value(name);

        url = json_object_get(data, "url");
        if (!json_is_string(url)) {
            fprintf(stderr, "config error: url #%d: url is not a string\n",
                    i + 1);
            json_decref(menu->root);
            exit(1);
        }
        url_text = json_string_value(url);

        menu->items[i] = (struct menu_item){.name = name_text, .url = url_text};
    }
}

struct menu menu_create(struct main_win *main_win, struct conf *conf) {
    WINDOW *win = newwin(main_win->height - 3, main_win->width, 0, 0);
    refresh();

    box(win, 0, 0);
    wrefresh(win);

    struct menu menu = (struct menu){
        .win = win,
        .height = main_win->height - 3,
        .width = main_win->width,
        .y = 0,
        .x = 0,
    };

    menu_load_conf(&menu, conf);

    int max =
        menu.items_len > menu.height - 2 ? menu.height - 2 : menu.items_len;
    menu.view_start = 0;
    menu.view_i = 0;
    menu.view_len = max;

    return menu;
}

void menu_bound(struct menu *menu) {
    if (menu->view_start > menu->items_len - menu->view_len) {
        menu->view_start = menu->items_len - menu->view_len;
    }

    if (menu->view_start < 0) {
        menu->view_start = 0;
    }
}

void menu_resize(struct menu *menu, struct main_win *main_win) {
    wclear(menu->win);
    wresize(menu->win, main_win->height - 3, main_win->width);
    box(menu->win, 0, 0);
    wrefresh(menu->win);

    menu->height = main_win->height - 3;
    menu->width = main_win->width;

    int max =
        menu->items_len > menu->height - 2 ? menu->height - 2 : menu->items_len;
    menu->view_len = max;
    menu->view_start = menu->view_i - max / 2;

    menu_bound(menu);
    menu_update(menu, -1);
}

void menu_scroll(struct menu *menu, enum direction direction) {
    if (menu->items_len > menu->view_len) {
        if (direction == UP) {
            if (menu->view_i > menu->view_len / 2) {
                menu->view_start++;
            }
        } else if (direction == DOWN) {
            if (menu->view_i < menu->items_len - menu->view_len / 2) {
                menu->view_start--;
            }
        }

        menu_bound(menu);
    }
}

struct menu_item *menu_update(struct menu *menu, int key) {
    switch (key) {
    case 'k':
        if (menu->view_i > 0) {
            menu->view_i--;
        }
        menu_scroll(menu, DOWN);
        break;

    case 'j':
        if (menu->view_i < menu->items_len - 1) {
            menu->view_i++;
        }
        menu_scroll(menu, UP);
        break;

    case KEY_CONFIRM:
        return &menu->items[menu->view_i];

    default:
        break;
    }

    for (int i = 0; i < menu->view_len; i++) {
        menu_clean(menu, i + 1);

        if (menu->view_start + i == menu->view_i) {
            wattron(menu->win, A_REVERSE);
        }
        mvwprintw(menu->win, 1 + i, 1, "%.*s", menu->width - 2,
                  menu->items[i + menu->view_start].name);
        wattroff(menu->win, A_REVERSE);
    }
    wrefresh(menu->win);

    return NULL;
}

void menu_destroy(struct menu *menu) {
    json_decref(menu->root);
    free(menu->items);
    delwin(menu->win);
}
