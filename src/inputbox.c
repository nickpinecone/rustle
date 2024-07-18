#include <ncurses.h>

#include "inputbox.h"

struct inputbox input_create(int y, int x, int width, char *label)
{
    WINDOW *win = newwin(3, width, y, x);
    refresh();

    box(win, 0, 0);
    mvwprintw(win, 0, 3, "%s", label);
    wrefresh(win);

    return (struct inputbox){
        .raw = win,
        .height = 3,
        .width = width,
        .y = y,
        .x = x,
        .inY = y + 1,
        .inX = x + 1,
        .content = "",
        .focus = false,
        .length = 0,
    };
}

void input_focus(struct inputbox *input)
{
    input->focus = true;
}

enum instatus input_capture(struct inputbox *input, int key)
{
    int posY = input->inY;
    int posX = input->inX + input->length;
    move(posY, posX);

    if (key != ERR)
    {
        switch (key)
        {
        case KEY_CONFIRM:
            input->focus = false;
            return Enter;
            break;

        case KEY_ESCAPE:
            input->focus = false;
            return Exit;
            break;

        case KEY_BACKSPACE:
            if (input->length > 0)
            {
                input->length--;
                input->content[input->length] = ' ';
            }
            break;

        default:
            if (input->length < 1000 && input->length < input->width - 2)
            {
                input->content[input->length] = (char)key;
                input->length++;
            }
            break;
        }

        mvprintw(input->inY, input->inX, "%s", input->content);
        wrefresh(input->raw);
        input->content[input->length] = '\0';
    }

    return Capture;
}
