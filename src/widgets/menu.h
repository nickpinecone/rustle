#ifndef MENU_H
#define MENU_H

#include <jansson.h>
#include <ncurses.h>

#include "../utils/conf.h"
#include "../utils/main_win.h"

enum direction {
    UP,
    DOWN,
};

struct menu_item {
    const char *name;
    const char *url;
};

struct menu {
    WINDOW *win;
    json_t *root;

    int height, width;
    int y, x;

    struct menu_item *items;
    int items_len;

    int view_len;
    int view_start;
    int view_i;
};

struct menu menu_create(struct main_win *main_win, struct conf *conf);
struct menu_item *menu_update(struct menu *menu, int key);
void menu_resize(struct menu *menu, struct main_win *main_win);
void menu_destroy(struct menu *menu);

#endif
