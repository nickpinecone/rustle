#ifndef PLAYERBOX_H
#define PLAYERBOX_H

#include <mpv/client.h>
#include <ncurses.h>

#include "selectmenu.h"

struct playerbox
{
    WINDOW *raw;
    int height, width;
    int y, x;

    mpv_handle *mpv;
    char name[1024];
    int length;
    bool pause;

    double prevolume;
};

struct playerbox player_create(int y, int x, int width);
void player_play(struct playerbox *player, struct selectitem *item);
void player_update(struct playerbox *player, int key);
void player_destroy(struct playerbox *player);

#endif
