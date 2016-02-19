# HiAssetBundle_unity
说明:在unity中实现热更新(包括资源更新,逻辑更新(需要自己添加lua))

(ps.如果项目中没有用到lua,则只会热更新资源部分)

完成功能:

1.维护本地lua文件与服务器一致.

2.维护本地assetbundle与服务器一致.

3.对assetbundle的常用操作(加载,卸载,处理依赖关系)

编辑器部分的功能:

1.lua打包,assetbundle打包,生成md5文件.

2.模拟热更新.

详细功能说明:

1.


注意事项:

1.文件/文件夹命名严禁包含空格(逻辑自动删除文件/文件夹名字中的空格)

1>从网络加载资源时

下载资源的url由目录地址和相对路径拼接而成,比如http://192.168.1.1/new test.txt

如果用www直接访问这个地址是访问不到的.

需要访问http://192.168.1.1/new%20test.txt才能访问到.

%20是html的空格编码.

2>访问本地streamingasset目录时,比如streamasset/new test.txt

直接访问这个地址就可以了.

为了保证一致,删除所有空格
