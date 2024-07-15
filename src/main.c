#include <ncurses.h>

struct window
{
    WINDOW *scr;
    int height, width;
    int y, x;
};

struct window main_win()
{
    struct window win;

    win.scr = stdscr;
    getyx(stdscr, win.y, win.x);
    getmaxyx(stdscr, win.height, win.width);

    return win;
}

struct window create_win(int height, int width, int y, int x)
{
    WINDOW *win = newwin(height, width, y, x);
    refresh();

    return (struct window){
        .scr = win,
        .height = height,
        .width = width,
        .y = y,
        .x = x,
    };
}

void box_win(struct window *win, int y, int x, const char *text)
{
    box(win->scr, 0, 0);
    mvwprintw(win->scr, y, x, "%s", text);
    wrefresh(win->scr);
}

void init()
{
    initscr();
    cbreak();
    noecho();
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

    struct window mainwin = main_win();
    struct window inputwin = create_win(3, mainwin.width, 0, 0);
    box_win(&inputwin, 0, 0, "Search");
    struct window selectwin = create_win(10, mainwin.width, inputwin.y + inputwin.height, 0);
    box_win(&selectwin, 0, 0, "Stations");

    char *items[] = {"Option 1", "Option 2", "Option 3"};
    int count = 3;
    int active = 0;

    char input[1024];
    int offset = 0;
    input[offset] = '\0';

    while (true)
    {
        for (int i = 0; i < count; i++)
        {
            if (i == active)
            {
                wattron(selectwin.scr, A_REVERSE);
            }
            mvwprintw(selectwin.scr, i + 1, 1, "%s", items[i]);
            wattroff(selectwin.scr, A_REVERSE);
        }

        wrefresh(selectwin.scr);
        char ch = getch();

        if (active == -1)
        {
            if (ch == ' ')
            {
                active = 0;
            }
            else
            {
                input[offset] = ch;
                offset++;
                input[offset] = '\0';
                mvwprintw(inputwin.scr, 1, 1, "%s", input);
                wrefresh(inputwin.scr);
            }
        }
        else
        {
            switch (ch)
            {
            case 'k':
                active--;
                if (active < 0)
                {
                    active = 0;
                }
                break;
            case 'j':
                active++;
                if (active >= count)
                {
                    active = count - 1;
                }
                break;
            case 'f':
                offset = 0;
                active = -1;
                move(inputwin.y + 1, inputwin.x + 1);
                break;
            }
        }
    }

    return close();
}
