#include <ncurses.h>

#include "inputbox.h"
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

    struct inputbox input = input_create(0, 0, 20, "Search");
    struct selectmenu menu = menu_create(input.y + input.height, 0, 10, 20, "Stations");

    menu_add(&menu, "Code Radio");
    menu_add(&menu, "Chillofi");
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

        menu_update(&menu, in);
    }

    return close();
}
