#ifndef MENU_H
#define MENU_H

#include <jansson.h>
#include <ncurses.h>

struct menu_item {
    const char *name;
    const char *url;
};

struct menu {
    WINDOW *win;
    json_t *root;

    int height;
    int width;
    int y;
    int x;

    struct menu_item *items;
    int items_len;

    struct menu_item **view;
    int view_len;
};

struct menu menu_create(int y, int x, int height, int width);
struct menu_item *menu_update(struct menu *menu, int key);
void menu_destroy(struct menu *menu);

#endif
