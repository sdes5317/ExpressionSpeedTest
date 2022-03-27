# ExpressionSpeedTest
由於在某個專案有動態賦值的需求  
故針對`使用Expression產生指定屬性的Set方法去賦值`  
與其他方法去做效能比較  

## 測試結果  
針對某個物件單個屬性做1000次的set  

|                                                              Method |            Mean |         Error |        StdDev |
|-------------------------------------------------------------------- |----------------:|--------------:|--------------:|
|                                 SetByExpressionNoCache1(Expression) |  106,845.943 us | 1,280.4211 us | 1,197.7067 us |
|        SetByExpressionAndCache(Expression+Action<object,obejct快取>) |       5.854 us |     0.0225 us |     0.0210 us |
|               SetByExpressionAndCacheDynamic(Expression+dynamic快取) |      20.272 us |     0.3530 us |     0.3302 us |
|                                                SetByReflection(反射) |      95.591 us |     0.5307 us |     0.4704 us |
|                                      SetByReflectionCache(反射+快取) |      20.019 us |     0.1480 us |     0.1385 us |
|                                           SetByNormal(正常用等號賦值) |       2.345 us |     0.0302 us |     0.0283 us |
|                                          SetByAutoMapper(快取Config) |      98.107 us |     0.3259 us |     0.2889 us |

  Mean   : Arithmetic mean of all measurements  
  Error  : Half of 99.9% confidence interval  
  StdDev : Standard deviation of all measurements  
  1 us   : 1 Microsecond (0.000001 sec)  

### 結論
```diff
- 基本的反射+快取就可以有好維護又不錯的效能
```

### 推薦閱讀清單
* [C# 筆記：Expression Trees][]

[C# 筆記：Expression Trees]: https://www.huanlintalk.com/2011/08/csharp-expression-trees.html
