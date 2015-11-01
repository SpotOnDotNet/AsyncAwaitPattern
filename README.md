## Materiały z pogadanki o async/await

### Uwagi do przykładów:
* Opisy zamieszczone są w kodzie.
* Przed wypróbowaniem przykładów należy uruchomić w tle projekt *ExternalService* - jest to usługa, którą wywołują fragmenty dotyczące asynchronicznego I/O.
* Na wyjściu debugowania wyświetlane są informacje o zmianach w liczbie wykorzystywanych *worker threads* oraz *completion port threads*.
* Brak należytej obsługi błędów oraz zwalniania obiektów implementujących *IDisposable* - celem zachowania prostoty.

### Polecane linki:
* Dla wszystkich: http://blog.stephencleary.com - kompendium wiedzy na temat programowania asynchroniczngo i współbieżnego w .NET.
* Dla niecierpliwych: https://msdn.microsoft.com/en-us/magazine/jj991977.aspx - zbiór dobrych praktyk w pigułce (oczywiście ich autorem jest również Stephen Cleary ;) ).
* Dla dociekliwych: http://weblogs.asp.net/kennykerr/parallel-programming-with-c-part-4-i-o-completion-ports - spojrzenie na mechanizm umożliwiający asynchroniczne operacje I/O.

### Tematy/problemy wiodące, związane z pogadanką:
* *Asynchronous Programming Model (APM)* vs *Task-based Asynchronous Pattern (TAP)* vs *Async/Await Pattern* 
* *I/O Completion Ports*
* *Task .NET 4.0* vs *Task .NET 4.5*
* *I/O processing* vs *CPU processing*
* *TaskScheduler.Default* vs *TaskScheduler.Current*
* *ExecutionContext* vs *SynchronizationContext*

### Kluczowe dobre praktyki programowania asychronicznego w C#:
* Stosowanie *async/await* w całym łańcuchu wywołań (od dołu do góry) - pozwoli uniknąć zakleszczeń oraz w pełni wykorzystać korzyści wydajnościowe płynące z zastosowania podejścia asynchronicznego.
* Rozsądne korzystanie z blokowań - jedynie w ostateczności, jeśli zachodzi potrzeba zblokowania wykonania w celu oczekiwania na zakończenie zadania, należy starać się to robić w pojedynczym, możliwie najwyżej położonym miejscu wykonania programu, korzystając z konstrukcji *.GetAwaiter().GetResult()*.
* Unikanie za wszelką cenę *async void* - konstrukcja ta nie pozwala na kontrolę nad wykonywanym zadaniem (np. zadanie takie nie może być częścią *Task.WhenAll*). Została stworzona do wykorzystania w asynchronicznych event handlerach i tylko w tym celu powinna być stosowana.
* Przywracanie kontekstu wywołania jedynie wtedy, gdy jest on wymagany - stosowanie konstrukcji *.ConfigureAwait(false)* w ramach konkretnej metody pozwala zwiększyć wydajność asynchronicznego kodu oraz uniknąć zakleszczeń gdy zachodzi konieczność zblokowania wywołania.
* Unikanie przeplatania API z .NET 4.0 i .NET 4.5 - metody klasy Task z .NET 4.5 pozwalają tworzyć kod wykonujący się bardziej przewidywalnie i w większym stopniu odporny na błędy, niż w przypadku metod z .NET 4 (*.GetAwaiter().GetResult()* zamiast maskującego wyjątki *.Result*, *Task.When** zamiast blokujących *Task.Wait**).
* Używanie *Thread* lub *Task.Run* do operacji obliczeniowych - *Thread* gdy tworzony wątek ma być spoza puli lub *Task.Run* gdy ma pochodzić z puli wątków.
* Wykorzystywanie *CancellationToken* do przerywania operacji asynchronicznej - asynchroniczne API wbudowane w *.NET Framework* wykorzystują ten mechanizm, warto również wykorzystywać go we własnym kodzie.
* Jak żyć? Kiedy stosować? Zawsze w przypadku operacji I/O oraz przy obliczeniach przeprowadzanych z wykorzystaniem *ThreadPool* (*Task.Run*).