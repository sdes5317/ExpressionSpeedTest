# ExpressionSpeedTest
測試set方法透過Expression與其他方法的效能比較

## 測試結果  
針對某個物件單個屬性做1000次的set  

|                                                              Method |          Mean |         Error |        StdDev |
|-------------------------------------------------------------------- |--------------:|--------------:|--------------:|
|                                 SetByExpressionNoCache1(Expression) | 59,979.196 us | 1,172.8174 us | 1,682.0197 us |
|        SetByExpressionAndCache(Expression+Action<object,obejct快取>) |      5.934 us |     0.0749 us |     0.0701 us |
|               SetByExpressionAndCacheDynamic(Expression+dynamic快取) |     21.512 us |     0.4273 us |     1.1552 us |
|                                                SetByReflection(反射) |    102.137 us |     2.0101 us |     3.7259 us |
|                                      SetByReflectionCache(反射+快取) |     20.433 us |     0.4069 us |     0.6686 us |
|                                           SetByNormal(正常用等號賦值) |      1.972 us |     0.0234 us |     0.0207 us |

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
