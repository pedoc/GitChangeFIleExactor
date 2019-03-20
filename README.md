# GitChangeFIleExactor
自动根据git commit记录，将变化的文件按照原目录结构输出到指定位置

支持命令  
-r 指定本地仓库目录  
-c 指定一个或多个commit id（或sha）  
-o 指定输出目录，如果目录不存在将自动创建  

示例命令  
GitChangeFIleExactor.exe -r H:TestProject -c 81a05e13 92d7057e  
