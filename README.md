# uwpRasp
Iot Project with test chart

<b>Introduction</b>
<br>
TO DO 
<br><br>
<b>Note</b>

```c#
Stopwatch sw = new Stopwatch();
sw.Start();

// Do something you want to time

sw.Stop();

long microseconds = sw.ElapsedTicks / (Stopwatch.Frequency / (1000L*1000L));
long nanoseconds = sw.ElapsedTicks / (Stopwatch.Frequency / (1000L*1000L*1000L));
```
