#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <sys/stat.h>
#include <sys/types.h>
#include <unistd.h>

#include "config.h"

char _home_dir[1024];
char _config_dir[1024];
char _mpv_path[1024];

char *_generate_mpv_config()
{
    return "stream-lavf-o-append=timeout=10000000\n\
stream-lavf-o-append=reconnect_on_http_error=4xx,5xx\n\
stream-lavf-o-append=reconnect_delay_max=30\n\
stream-lavf-o-append=reconnect_streamed=yes\n\
stream-lavf-o-append=reconnect_on_network_error=yes\n";
}

void config_init()
{
    strcpy(_home_dir, getenv("HOME"));
    sprintf(_config_dir, "%s/.config/riff/", _home_dir);
    sprintf(_mpv_path, "%s/mpv.conf", _config_dir);

    if (stat(_config_dir, &(struct stat){}) == -1)
    {
        mkdir(_config_dir, S_IRWXU);
        FILE *mpv_conf = fopen(_mpv_path, "w");
        fprintf(mpv_conf, "%s", _generate_mpv_config());
        fclose(mpv_conf);
    }
}

char *config_get_mpv()
{
    return _mpv_path;
}
