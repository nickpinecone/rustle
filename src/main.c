#include <mpv/client.h>
#include <ncurses.h>

#include "widgets/inputbox.h"
#include "widgets/keycodes.h"
#include "widgets/selectmenu.h"

void init()
{
    initscr();
    cbreak();
    noecho();
    keypad(stdscr, TRUE);

    // Non blocking input
    timeout(0);
}

int close()
{
    endwin();
    return 0;
}

int main()
{
    init();

    mpv_handle *player = mpv_create();
    mpv_initialize(player);

    struct inputbox input = input_create(0, 0, 20, "Search");
    struct selectmenu menu = menu_create(input.y + input.height, 0, 10, 20, "Stations");

    menu_add(&menu, "Code Radio", "https://coderadio-admin-v2.freecodecamp.org/listen/coderadio/radio.mp3");
    menu_update(&menu, ERR);

    bool isOn = true;
    input_focus(&input);
    while (isOn)
    {
        int in = getch();

        switch (input_capture(&input, in))
        {
        case Enter:
            isOn = false;
            break;

        case Exit:
            isOn = false;
            break;

        case Capture:;
        }
    }

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
    return close();
}
