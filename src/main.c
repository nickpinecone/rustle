#include <ncurses.h>

#include "selectmenu.h"


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

    struct selectmenu menu = menu_create(0, 0, 10, 20, "Stations");

    menu_add(&menu, "Code Radio");
    menu_add(&menu, "Chillofi");

    while (true)
    {
        int in = getch();

        menu_update(&menu, in);
    }

    // struct inputbox input = input_create(0, 0, 20, "Search");
    //
    // bool isOn = true;
    // while (isOn)
    // {
    //     int in = getch();
    //     input_focus(&input);
    //
    //     switch (input_capture(&input, in))
    //     {
    //     case Enter:
    //         isOn = false;
    //         break;
    //
    //     case Exit:
    //         isOn = false;
    //         break;
    //
    //     case Capture:;
    //     }
    // }

    return close();
}
