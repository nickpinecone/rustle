#include <ncurses.h>

#include "utils/conf.h"
#include "utils/main_win.h"
#include "widgets/menu.h"
#include "widgets/player.h"

int main() {
    struct conf conf = conf_init();
    struct main_win main_win = main_win_init();
    struct menu menu = menu_create(&main_win, &conf);
    struct player player = player_create(&main_win, &conf);

    int key = 0;

    while (true) {
        struct menu_item *item = menu_update(&menu, key);

        if (item != NULL) {
            player_play(&player, item);
        }

        player_update(&player, key);

        if (key == 'q') {
            break;
        } else if (key == KEY_RESIZE) {
            main_win_resize(&main_win);
            menu_resize(&menu, &main_win);
            player_resize(&player, &main_win);
        }

        key = getch();
    }

    conf_destroy(&conf);
    menu_destroy(&menu);
    player_destroy(&player);
    return main_win_close();
}
