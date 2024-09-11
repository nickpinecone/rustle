#include <mpv/client.h>
#include <ncurses.h>
#include <string.h>

#include "../utils/conf.h"
#include "keys.h"
#include "player.h"

char blank2[1024];

void stop(struct player *player, bool clear) {
    if (clear) {
        strcpy(player->name, "");
    }

    const char *args[] = {"stop", NULL};
    mpv_command(player->mpv, args);
}

void volume_down(struct player *player) {
    const char *args[] = {"add", "volume", "-1", NULL};
    mpv_command(player->mpv, args);
}

void volume_up(struct player *player) {
    const char *args[] = {"add", "volume", "+1", NULL};
    mpv_command(player->mpv, args);
}

void toggle(struct player *player) {
    player->paused = !player->paused;
    char volume[1024];

    if (player->paused) {
        mpv_get_property(player->mpv, "volume", MPV_FORMAT_DOUBLE,
                         &player->prevolume);
        strcpy(volume, "0");
    } else {
        sprintf(volume, "%.f", player->prevolume);
    }

    const char *args[] = {"set", "volume", volume, NULL};
    mpv_command(player->mpv, args);
}

struct player player_create(int y, int x, int width) {
    mpv_handle *mpv = mpv_create();

    mpv_load_config_file(mpv, conf_get_mpv());
    mpv_initialize(mpv);

    WINDOW *win = newwin(3, width, y, x);
    refresh();

    box(win, 0, 0);
    wrefresh(win);

    memset(blank2, ' ', sizeof(char) * width - 2);

    return (struct player){.win = win,
                           .y = y,
                           .x = x,
                           .width = width,
                           .height = 3,
                           .mpv = mpv,
                           .paused = true,
                           .prevolume = 100};
}

void player_play(struct player *player, struct menu_item *item) {
    stop(player, false);

    strcpy(player->name, item->name);
    player->paused = false;

    const char *args[] = {"loadfile", item->url, NULL};
    mpv_command(player->mpv, args);
}

void player_update(struct player *player, int key) {
    switch (key) {
    case KEY_ESCAPE:
        stop(player, true);
        break;

    case ' ':
        toggle(player);
        break;

    case 'n':
        volume_down(player);
        break;

    case 'p':
        volume_up(player);
        break;

    default:
        break;
    }

    mvwprintw(player->win, 1, 1, "%s", blank2);
    mvwprintw(player->win, 1, 1, "%s", player->name);

    if (strcmp(player->name, "") != 0) {
        wprintw(player->win, " ");
    }

    double volume;
    mpv_get_property(player->mpv, "volume", MPV_FORMAT_DOUBLE, &volume);
    wprintw(player->win, "%.f", volume);
    wrefresh(player->win);
}

void player_resize(struct player *player, int height, int width) {
    wclear(player->win);
    wresize(player->win, 3, width);
    mvwin(player->win, height - 3, 0);
    box(player->win, 0, 0);
    wrefresh(player->win);

    memset(blank2, '\0', sizeof(char) * 1024);
    memset(blank2, ' ', sizeof(char) * width - 2);

    player_update(player, -1);
}

void player_destroy(struct player *player) {
    mpv_destroy(player->mpv);
    delwin(player->win);
}
