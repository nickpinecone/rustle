#include <mpv/client.h>
#include <ncurses.h>

#include "utils/mainwindow.h"
#include "widgets/keycodes.h"
#include "widgets/selectmenu.h"

int main()
{
    struct mainwindow main = main_init();
    mpv_handle *player = mpv_create();
    mpv_initialize(player);

    struct selectmenu menu = menu_create(0, 0, main.height - 3, main.width - 1, "Stations");
    menu_add(&menu, "Code Radio", "https://coderadio-admin-v2.freecodecamp.org/listen/coderadio/radio.mp3");

    menu_focus(&menu);
    while (true)
    {
        int in = getch();

        if (in == KEY_ESCAPE)
        {
            break;
        }
        else if (in == ' ')
        {
            const char *args[] = {"stop", NULL};
            mpv_command(player, args);
        }

        struct selectitem *item = menu_update(&menu, in);

        if (item != NULL)
        {
            const char *args[] = {"loadfile", item->url, NULL};
            mpv_command(player, args);
        }
    }

    mpv_destroy(player);
    return main_close();
}
