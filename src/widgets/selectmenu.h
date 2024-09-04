#ifndef SELECTMENU_H
#define SELECTMENU_H

#include <ncurses.h>

struct selectitem
{
    int order;
    char label[1024];
    int length;

    // Station url
    char url[1024];
};

struct selectmenu
{
    WINDOW *raw;
    int height, width;
    int y, x;

    bool focus;
    int active;
    int count;
    struct selectitem *items[16];
};

struct selectmenu menu_create(int y, int x, int height, int width, char *label);
void menu_add(struct selectmenu *menu, char *label, char *url);
void menu_focus(struct selectmenu *menu);
struct selectitem *menu_update(struct selectmenu *menu, char key, char *filter);
void menu_destroy(struct selectmenu *menu);

#endif
