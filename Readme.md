# Rim World Translator
# 边缘世界翻译工具
Github：https://github.com/inkitter/RinTrans
> 需要安装.NET Framework 4.5。

## v1.0.0

## 根据之前的项目pdx-ymltranslator，稍作修改，创建了 rimworld 的汉化工具。
* 运行前准备：在 exe 程序文件夹建立 Mods 文件夹，结构、内容与 游戏文件夹\Mods 相同。可使用下面附带的dos命令，将其中路径改为自己对应的游戏路径即可。
* 暂时不支持从 def 中提取英文文本。
* 其他功能详见 https://github.com/inkitter/pdx-ymltranslator。
* Bug目前较多

### 可能用到的dos命令
```
rd Mods /s /q
mklink /j Mods "D:\SteamLibrary\steamapps\common\RimWorld\Mods"

```