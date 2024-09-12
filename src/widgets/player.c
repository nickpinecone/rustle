#include <mpv/client.h>
#include <ncurses.h>
#include <string.h>

#include "keys.h"
#include "player.h"

void player_clean(struct player *player) {
    char blank[player->width - 1];
    memset(blank, ' ', player->width - 2);
    blank[player->width - 2] = '\0';
    mvwprintw(player->win, 1, 1, "%s", blank);
}

void player_stop(struct player *player, bool clear) {
    if (clear) {
        player->name = "";
    }

    const char *args[] = {"stop", NULL};
    mpv_command(player->mpv, args);
}

void player_volume_down(struct player *player) {
    const char *args[] = {"add", "volume", "-1", NULL};
    mpv_command(player->mpv, args);
}

void player_volume_up(struct player *player) {
    const char *args[] = {"add", "volume", "+1", NULL};
    mpv_command(player->mpv, args);
}

void player_toggle(struct player *player) {
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

struct player player_create(struct main_win *main_win, struct conf *conf) {
    mpv_handle *mpv = mpv_create();

    mpv_load_config_file(mpv, conf->mpv_path);
    mpv_initialize(mpv);

    WINDOW *win = newwin(3, main_win->width, main_win->height - 3, 0);
    refresh();

    box(win, 0, 0);
    wrefresh(win);

    return (struct player){.win = win,
                           .y = main_win->height - 3,
                           .x = 0,
                           .width = main_win->width,
                           .height = 3,
                           .mpv = mpv,
                           .name = "",
                           .paused = true,
                           .prevolume = 100};
}

void player_play(struct player *player, struct menu_item *item) {
    player_stop(player, false);

    player->name = item->name;
    player->paused = false;

    const char *args[] = {"loadfile", item->url, NULL};
    mpv_command(player->mpv, args);
}

void player_update(struct player *player, int key) {
    switch (key) {
    case KEY_ESCAPE:
        player_stop(player, true);
        break;

    case ' ':
        player_toggle(player);
        break;

    case 'n':
        player_volume_down(player);
        break;

    case 'p':
        player_volume_up(player);
        break;

    default:
        break;
    }

    player_clean(player);
    mvwprintw(player->win, 1, 1, "%s", player->name);

    if (strcmp(player->name, "") != 0) {
        wprintw(player->win, " ");
    }

    double volume;
    mpv_get_property(player->mpv, "volume", MPV_FORMAT_DOUBLE, &volume);
    wprintw(player->win, "%.f", volume);
    wrefresh(player->win);
}

void player_resize(struct player *player, struct main_win *main_win) {
    wclear(player->win);
    wresize(player->win, 3, main_win->width);
    mvwin(player->win, main_win->height - 3, 0);
    box(player->win, 0, 0);
    wrefresh(player->win);

    player->width = main_win->width;
    player->y = main_win->height - 3;
    player_update(player, -1);
}

void player_destroy(struct player *player) {
    mpv_destroy(player->mpv);
    delwin(player->win);
}
