# Scroll

### Command line arguments

```

  -i, --input       Required. Input file containing region vertices

  -o, --output      Required. Output folder

  -c, --combined    Generate map atlas

  --min             Minimum scale level

  --max             Maximum scale level

  -l, --level       Certain scale level

  -s, --scaler      Scaler for map tile

```

### Example

下载层级14-18的瓦片，并将瓦片合成为一个地图。

```./Scroll.exe -i ./input.txt --min 14 --max 18 -c```

下载层级为18，大小为512x的瓦片

```./Scroll.exe -i ./input.txt -l 18 -s 2```

```Scroll/input.txt```中为华科附近坐标