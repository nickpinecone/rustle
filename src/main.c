#include <ncurses.h>

#include "inputbox.h"

void init()
{
    initscr();
    cbreak();
    noecho();
    keypad(stdscr, TRUE);

    // Non blocking input
    timeout(100);
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

    bool isOn = true;
    while (isOn)
    {
        int in = getch();
        input_focus(&input);

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

    return close();
}
