# BWikiImgRecoder

### 使用说明

``
.\BWikiImgRecoder compress -i [源文件路径] -o [输出路径] -f [输出格式] -l [大小限制(MB)] -d [递归深度限制] -s [压缩步进] --force [强制压缩]
``

必选参数:

-i 源文件路径, 字符串数组, 可以接受多个路径, 以空格分隔.

可选参数:

-o 输出路径, 字符串, 唯一参数, 默认为程序当前路径下Output\Result_

-f 输出格式, 数字, 唯一参数, 仅可选值 PNG:0 / JPEG:1, 默认为1

-l 大小限制, 数字, 唯一参数, 默认为8

-d 递归深度限制, 数字, 唯一参数, 限制当源过大需要多次递归缩小时最多递归次数, 默认为50

-s 压缩步进, 数字, 唯一参数, 当压缩大小接近限制大小时缩小步进, 默认为50

--force 强制压缩, 布尔, 唯一参数, 当源符合限制大小时是否强制执行压缩/修改格式, 默认为false

#### 示例

``
.\BWikiImgRecoder compress -i D:\1.png F:\2.png -o F:\out 
``

``
.\BWikiImgRecoder compress -i D:\1.png F:\2.png -o F:\out -l 10 -d 50 -s 20 --force
``