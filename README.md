HiAssetBundle_unity
===================
Put the whole asset bundles collections on a web server, then will automatically maintain local resources as server be.

Web服务器放置一份完整asset bundle资源，自动维护本地资源与服务器一致。


----------
**功能说明:**

> - 资源重命名(鼠标选在文件夹,点击编辑器菜单下的重命名按钮,会重命名该文件夹下的所有文件)
> -  快速设置AssetBundle的包名(点击文件夹,会自动把该文件夹下资源的ab的包名设置为文件夹的名字)
> - 打包Assetbundle资源包(支持windows/android/ios)
> - 资源统一加载逻辑(只需要传入文件名就能从对应的assetbundle中加载对应的文件)
> - 资源在内存中的释放
> - 跟包资源(打包到手机安装包的资源)解压到可读写目录
> - 提示需要下载XX兆更新,确定下载吗?
> - 下载服务器最新资源(热更),保持本地资源与服务器资源一致

----------
**详细说明:**
> - 资源重命名后会包含路径信息,为以后assetbundle加载资源方便(不需要在提供包名+文件名)
> - 编辑器模式下使用AssetDatabase方式加载资源(为方便快速开发迭代)
> - 编辑器菜单的assetbundle命名只是傻瓜式命名(以每个prefab的文件夹作为包名),需要将公共资源设置到统一的assetbundle中
> - 打包逻辑默认采用非压缩方式,可手动修改源码改为其他方式
> - 打包逻辑会将生成的assetbundle文件放置到StreamingAsset目录下(方便初始化运行时复制资源)
> - 打包逻辑主要分unity引擎维护部分(处理依赖关系)和其他部分(文本文件,数据库文件,二进制文件等)
> - 哈希文件:
>  - 执行打包逻辑会生成哈希列表,包含哈希值,文件存储目录,文件大小.
>  - 通过获取哈希列表中的文件大小能够实现: "您有10M资源要更新,需要现在更新吗"
>  - 通过哈希文件比对能获得所要更新的文件数量和进度条:"您正在更新第5个文件"
>   - 一共有100个文件需要更新,当更新到第50个时网络断开,下次继续从第51个开始更新
> - 资源加载提供接口,会通过传入的文件名解析获得相应的资源(编辑器下直接获取,非编辑器从assetbundle中获取)
> - 初始化时会自动启动资源依赖关系处理逻辑,加载物体会加载依赖的资源,卸载物体会卸载依赖的资源(如果依赖资源未被其他资源依赖进行卸载,如果被其他资源依赖,继续在内存中)
> - 提供接口主动卸载assetbundle在内存中的占用

----------
**操作流程:**
 > - 编辑器模式下(方便测试)
 >  - 点击build assetbundle window
 >   - 运行unity即可
 > - 非编辑器模式(正式流程)
 >  - 设置文件名(编辑器已提供按钮)
 >  - 设置assetbundle包名(编辑器已提供按钮)
 >  - 点击build assetbundle android(选择自己对应的平台)
 >  - 发布apk,ipa文件
 
----------
**逻辑执行流程:**
> - 将跟包资源(安装包中的资源)复制到可读写目录下
> - 复制完成开始比对哈希文件,开始更新资源
> - 下载最添加资源,替换旧资源,删除原来可读写目录下的无用资源
> - 初始化assetbundle依赖关系
> - 完成整个流程

----------
**逻辑热更篇:**
> - 无论是L#更新dll还是使用ulua,tolua,slua更新.lua文件都可以当做是资源文件处理.
> - 能成功下载到最新的.dll或.lua,热更实现参照对应的官网文档.

----------
> **Note:**

> - 文件/文件夹命名严禁包含空格(逻辑自动删除文件/文件夹名字中的空格)

>  - 从网络加载资源时下载资源的url由目录地址和相对路径拼接而成,比如http://192.168.1.1/new test.txt如果用www直接访问这个地址是访问不到的.需要访问http://192.168.1.1/new%20test.txt才能访问到.%20是html的空格编码.

>  - 访问本地streamingasset目录时,比如streamasset/new test.txt直接访问这个地址就可以了.为了保证一致,删除所有空格
> - 注意unix系统(android/ios)的大小写敏感问题


support: hiramtan@live.com



MIT License

Copyright (c) [2017] [Hiram]

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
