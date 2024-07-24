#include <mpv/client.h>
#include <ncurses.h>

#include "utils/mainwindow.h"
#include "widgets/playerbox.h"
#include "widgets/selectmenu.h"

int main()
{
    struct mainwindow main = main_init();
    struct selectmenu menu = menu_create(0, 0, main.height - 3, main.width, "Stations");
    struct playerbox player = player_create(menu.y + menu.height, 0, main.width);

    menu_add(&menu, "Code Radio", "https://coderadio-admin-v2.freecodecamp.org/listen/coderadio/radio.mp3");
    menu_add(&menu, "Chillofi", "http://streams.dez.ovh:8000/radio.mp3");
    menu_focus(&menu);

    int in = ' ';
    while (true)
    {
        if (in != ERR)
        {
            if (in == 'q')
            {
                break;
            }

            struct selectitem *item = menu_update(&menu, in);

            if (item != NULL)
            {
                player_play(&player, item);
            }

            player_update(&player, in);
        }

        in = getch();
    }

    return main_close();
}
