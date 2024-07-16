#include <ncurses.h>

#include "inputbox.h"

void init()
{
    initscr();
    cbreak();
    noecho();
    keypad(stdscr, TRUE);
}

int close()
{
    getch();
    endwin();
    return 0;
}

int main()
{
    init();

    struct inputbox input = input_create(3, 20, 0, 0);
    refresh();
    input_box(&input, "Search", 0, 3);
    wrefresh(input.raw);

    bool isOn = true;
    while (isOn)
    {
        switch (input_capture(&input))
        {
        case Exit:
            isOn = false;
            break;
        default:
            continue;
        }
        refresh();
        wrefresh(input.raw);
    }

    mvprintw(input.y + input.height, 0, "Entered: %s", input.content);

    return close();
}
