#include "inputbox.h"

#include <ncurses.h>

struct inputbox input_create(int height, int width, int y, int x)
{
    WINDOW *win = newwin(height, width, y, x);

    return (struct inputbox){
        .raw = win,
        .height = height,
        .width = width,
        .y = y,
        .x = x,
        .inX = x,
        .inY = y,
        .content = "",
        .isFocus = false,
        .length = 0,
    };
}

void input_box(struct inputbox *input, char *label, int y, int x)
{
    box(input->raw, 0, 0);
    mvwprintw(input->raw, y, x, "%s", label);

    input->inY = input->y + 1;
    input->inX = input->x + 1;
}

enum instate input_capture(struct inputbox *input)
{
    input->isFocus = true;

    int posY = input->inY;
    int posX = input->inX + input->length;
    move(posY, posX);

    int in = getch();

    switch (in)
    {
    case '\n':
        input->isFocus = false;
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
            input->content[input->length] = (char)in;
            input->length++;
        }
        break;
    }

    mvprintw(input->inY, input->inX, "%s", input->content);
    wrefresh(input->raw);
    input->content[input->length] = '\0';

    return Capture;
}
