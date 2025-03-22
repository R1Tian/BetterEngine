# 通用库
把其他库有复用的都放这里

## Generic
通用类，命名空间Bamboo  
TypeCache：typeof缓存，避免计算多次。

## Helper
通用静态Helper，命名空间Bamboo  
EditorSettingHelper: 提供获取唯一Setting，发布后不可用。
ReflectionHelper：C#反射相关，尽量少调用，最好只在类库初始化时调用一次。

## CodeTool
代码工具  
Bamboo/Common/编码标准化：把脚本格式转换成UTF8，并把Tab换成空格。  
CodeBuilder：封装StringBuilder，用来程序生成代码。

## Imgui
对UnityEditor二次封装  
AdvanceDropDown: 下拉框扩展。  
BInspector：封装UnityEditor.Editor，提供快速Inspector绘制。

## HeartBeat
心跳Update  
UpdateTimeDriver：Update驱动，x毫秒  
UpdateFrameDriver：Update驱动，1帧  
FixedUpdateTimeDriver：FixedUpdate驱动，x毫秒  
FixedUpdateFrameDriver：FixedUpdate驱动，1帧

## NumberString
提供int和string的快速互转，内部使用池，多次调用无GC。
string str = 1.FastToString();
int i = "1".FastToInt();