#ifndef PLAYER_H
#define PLAYER_H

#include <mpv/client.h>
#include <ncurses.h>

#include "../utils/conf.h"
#include "../utils/main_win.h"
#include "menu.h"

struct player {
    WINDOW *win;

    int height, width;
    int y, x;

    mpv_handle *mpv;
    const char *name;

    bool paused;
    double prevolume;
};

struct player player_create(struct main_win *main_win, struct conf *conf);
void player_play(struct player *player, struct menu_item *item);
void player_update(struct player *player, int key);
void player_resize(struct player *player, struct main_win *main_win);
void player_destroy(struct player *player);

#endif
