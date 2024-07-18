#include <ncurses.h>
#include <stdlib.h>
#include <string.h>

#include "selectmenu.h"

struct selectmenu menu_create(int y, int x, int height, int width, char *label)
{
    WINDOW *win = newwin(height, width, y, x);
    refresh();

    box(win, 0, 0);
    mvwprintw(win, 0, 3, "%s", label);
    wrefresh(win);

    return (struct selectmenu){.raw = win, .height = height, .width = width, .y = y, .x = x, .active = -1, .count = 0};
}

void menu_add(struct selectmenu *menu, char *label)
{
    struct selectitem *item = (struct selectitem *)malloc(sizeof(struct selectitem));
    strcpy(item->label, label);
    item->length = strlen(label);

    item->order = menu->count + 1;
    if (menu->active == -1)
    {
        menu->active = 0;
    }

    menu->items[menu->count] = item;
    menu->count++;
}

void menu_update(struct selectmenu *menu, char key)
{
    for (int i = 0; i < menu->count; i++)
    {
        if (i == menu->active)
        {
            wattron(menu->raw, A_REVERSE);
        }
        mvwprintw(menu->raw, i + 1, 1, "%s", menu->items[i]->label);
        wattroff(menu->raw, A_REVERSE);
    }

    struct selectitem *selected = menu->items[menu->active];
    int posX = menu->x + 1 + selected->length;
    int posY = menu->y + 1 + menu->active;
    wmove(menu->raw, posY, posX);

    wrefresh(menu->raw);

    switch (key)
    {
    case 'k':
        menu->active--;
        if (menu->active < 0)
        {
            menu->active = 0;
        }
        break;
    case 'j':
        menu->active++;
        if (menu->active >= menu->count)
        {
            menu->active = menu->count - 1;
        }
        break;
    default:
        break;
    }
}
