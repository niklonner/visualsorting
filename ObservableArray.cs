using System;

public class CompareEvent<T> : EventArgs {
       public CompareEvent(T fst, T snd) {
              this.fst = fst;
              this.snd = snd;
       }
       
       public T fst { get; private set;}
       public T snd { get; private set;}
}

public class AssignEvent<T> : EventArgs {
       public AssignEvent(int index, T obj) {
              this.index = index;
              this.obj = obj;
       }

       public int index { get; private set; }
       public T obj { get; private set; }
}

public class ObservableArray<T> where T : IComparable<T>  {
       private Wrapper[] array;

       public event EventHandler<CompareEvent<T>> Compare;
       public event EventHandler<AssignEvent<T>> Assign;

       public T this[int i] {
              get {
                  checkBounds(i);
                  return array[i].wrapped;
              }
              set {
                  checkBounds(i);
                  fireAssign(new AssignEvent<T>(i, value));
                  array[i] = new Wrapper(value, this);
              }
       }

       private void fireCompare(CompareEvent<T> args) {
               if (Compare != null) {
                  Compare(this, args);
               }
       }

       private void fireAssign(AssignEvent<T> args) {
               if (Assign != null) {
                  Assign(this, args);
               }
       }
       
       private void checkBounds(int i) {
               if (i < 0 || i >= array.Length) {
                  throw new ArgumentOutOfRangeException();
               }
       }
       
       public ObservableArray(int size) {
              array = new Wrapper[size];
       }

       public ObservableArray(T[] arr) {
              array = new Wrapper[arr.Length];
              int i=0;
              foreach (T t in arr) {
                    this[i++] = t;
              }
       }
       
       protected class Wrapper : IComparable<Wrapper> {
             public T wrapped;
             private ObservableArray<T> outer;

             public Wrapper (T val, ObservableArray<T> outer) {
                    wrapped = val;
                    this.outer = outer;
             }

             public int CompareTo(Wrapper other) {
                    outer.fireCompare(new CompareEvent<T>(wrapped, other.wrapped));
                    return wrapped.CompareTo(other.wrapped);
             }
       }

}

public static class Sorting {
    public static void insertionSort<T>(ObservableArray<T> array) where T : IComparable<T> {
                
    }
}

public class MainClass {
       public static void Main(String[] args) {
             ObservableArray<int> arr = new ObservableArray<int>(new int[] {2,3,5});
             
             Sorting.insertionSort(arr);
             
       }
}

public interface ArrayView<T> {
    void init();
    EventHandler<CompareEvent<T>> compareHandler {get;}
    EventHandler<AssignEvent<T>> assignHandler {get;}
}

public class Controller<T> where T : IComparable<T> {
    private ObservableArray<T> arr;
    private ArrayView<T> view;
    
    public Controller(ObservableArray<T> arr, ArrayView<T> view) {
        this.arr = arr;
        this.view = view;
        arr.Compare += view.compareHandler;
        arr.Assign += view.assignHandler;
    }        
}

public class ASCIIView<T> : ArrayView<T> {
    public void init() {
    
    }
    
    public EventHandler<CompareEvent<T>> compareHandler { get; private set; }
    public EventHandler<AssignEvent<T>> assignHandler { get; private set; }
    
    public ASCIIView() {
        compareHandler = compareEvent;
        assignHandler = assignEvent;
    }
    
    public void compareEvent(Object sender, CompareEvent<T> e) {
    
    }
    
    public void assignEvent(Object sender, AssignEvent<T> e) {
    
    }
}


