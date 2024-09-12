#ifndef CONF_H
#define CONF_H

struct conf {
    char *mpv_path;
    char *riff_path;
};

struct conf conf_init();
void conf_destroy(struct conf *conf);

#endif
