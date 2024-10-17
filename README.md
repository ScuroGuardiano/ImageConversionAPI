# ImageConversionAPI

Simple image conversion API using SixLabors.ImageSharp library. It's very simple, has two endpoints:
* `GET /convert`: returns `string[]` containing supported output formats.
* `POST /convert` - converts an image to the target format:
  * Form: `file` - file to convert
  * Query:
    * `w` - `int32` - width of output image between 0 and 60000.
    * `h` - `int32` - height of output image between 0 and 60000.
    * `q` - `int32` - quality of output image between 0 and 100. Only for supported formats.
    * `f` - `string` - format.

Note: this is very simple API. It doesn't have any rate limit, no HTTPS etc., so if you want to expose it you should do it through reverse proxy like nginx or caddy.  
I will add some logs and metrics later, maybe ^^

## Running with docker:
```shell
docker-compose up -d
```

Edit `docker-compose.yml` to change exposed port.

# LICENSE
AGPLv3