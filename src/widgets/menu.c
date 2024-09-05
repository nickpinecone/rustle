#include <jansson.h>
#include <ncurses.h>
#include <stdio.h>

#include "../utils/conf.h"
#include "menu.h"

void load_conf(struct menu *menu) {
    json_error_t error;

    menu->root = json_load_file(conf_get_riff(), 0, &error);

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

struct menu menu_create(int y, int x, int height, int width) {
    WINDOW *win = newwin(height, width, y, x);
    refresh();

    box(win, 0, 0);
    wrefresh(win);

    struct menu menu = (struct menu){
        .win = win,
        .height = height,
        .width = width,
        .y = y,
        .x = x,
    };
    load_conf(&menu);

    int max = menu.items_len > height ? height : menu.items_len;
    menu.view = malloc(sizeof(struct menu_item *) * max);
    menu.view_len = max;

    for (int i = 0; i < max; i++) {
        menu.view[i] = &menu.items[i];
    }

    return menu;
}

struct menu_item *menu_update(struct menu *menu, int key) {
    switch (key) {
    case 'k':
        break;

    case 'j':
        break;

    case KEY_ENTER:
        break;

    default:
        break;
    }

    for (int i = 0; i < menu->view_len; i++) {
        mvwprintw(menu->win, 1 + i, 1, "%s", menu->view[i]->name);
    }
    wrefresh(menu->win);

    return NULL;
}

void menu_destroy(struct menu *menu) {
    json_decref(menu->root);
    free(menu->items);
    free(menu->view);
    delwin(menu->win);
}
