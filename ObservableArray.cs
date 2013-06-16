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
                  array[i] = new Wrapper(value);
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
             public Wrapper (T val) {
                    wrapped = val;
             }

             public int CompareTo(Wrapper other) {
                    return wrapped.CompareTo(other.wrapped);
             }
       }

}

public class MainClass {
       public static void Main(String[] args) {
 //             ObservableArray<int> arr = new ObservableArray<int>();
       }
}