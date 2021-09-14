# NetPtr Specification

This specification covers the network pointer(NetPtr) and its associated binary format.

> Note:
> Despite the name a NetPtr does not work as a normal pointer and many pointer operations can not be performed with this object

## Binary Format

The NetPtr is a binary representation of both a unique identifier to a by-reference networked object, and the type of object being pointed to.

### Types of Managed objects

There are several types a net ptr may point to on a remote client or server.

* Unmanaged Reference
  * Examples:
    * `ref int`, `ref float`, `ref DateTime`, `ref Decimal`
    * `ref unmanaged struct`
* Unmanaged Array
  * Examples:
    * `int[]`, `float[]`, `DateTime[]`
    * `unmanaged struct[]`
* Complex Type
  * Examples:
    * User-Defined class or object
* Complex Type Member
  * Property, Field, Method
* Complex Type Array Element
  * Property or field that is an array

Each managed network object is encoded into a `ulong` so objects can be referenced without network communication to determine the object being pointed to.

The binary format is seperated into 3 fields, "Upper Bits", "Mid Bits", and "Lower Bits".

```csharp
// hex representation of a ulong
FFFF_FFFF_FFFF_FFFF

// upper bits (leading bits)
FFFF_0000_0000_0000

// mid bits
0000_FFFF_0000_0000

// lower bits (trailing bits)
0000_0000_FFFF_FFFF
```

#### Net Ptr Limitations

Because of the vast information encoded into a single pointer ( to limit network traffic and for performance reasons) there are hard limits on the amount of referenced objects on the managed network heap.

| Object Type | Maximum | Limitation Kind |
--- | --- | --
|unmanaged|2,147,483,647|global
|unmanaged array|65,535|global
|unmanaged array length|2,147,483,647| per-instance
|complex user-defined|65,535|global
|complex member|65,535|per-instance
|unmanaged array elements stored within complex type member|2,147,483,647|per-member

```csharp
// 0 - 2,147,483,647
NetPtr ptr = NetAlloc<int>(12);

// 0 - 65,535
NetPtr ptr = NetAlloc<int[]>(new int[<0 - int.MaxValue>]);

// 0 - 65,535
NetPtr ptr = NetAlloc<MyClass>(new MyClass()
{
    // 0 - 65,535
    Name = new char[<0 - int.MaxValue>],
});

// 0 - 2,147,483,647
NetPtr ptr = NetAlloc<MyUnmanagedStruct>(new MyUnmanagedStruct()
{
    // 0 - 65,535
    Name = new char[<0 - int.MaxValue>],
});
```

#### Null NetPtr

A null network pointer, a pointer that points to nothing, is encoded as 0

```csharp
// null net ptr
0000_0000_0000_0000
```

#### Unmanaged References

Unmanaged References that are not associated with a complex object(think global variables, if you'rem into that kind of thing) are encoded in lower bits. There can be a maximum number of `int.MaxValue` unmanaged references as they are encoded in a `32bit` space within the ptr.

```csharp
// unmanaged reference pointer format
0000_0000_FFFF_FFFF
```

When objects are stored on the network managed heap they are provided with a pointer, of the pointer has no leading or mid bits, the pointer must reference a by-reference unmanaged object on the heap.

#### Unamanaged Arrays

Unamanged arrays that are not associated with a complex type are encoded with no leading bits. If no trailing bits exist the pointer points to the first element of the unmanaged array.

```csharp
// unmanaged array format
//   array    index
0000_FFFF_FFFF_FFFF

// unmanaged array int[]
0000_0001_0000_0000

// unmanaged array int[1]
0000_0001_0000_0001
```

#### Complex Types

A network pointer to a complex type is identified by the complex type instance pointer which is encoded into the leading bits of the pointer. The mid bits of the pointer identify the member which is being pointed to and the trailing bits, if present, identify which index within the member(if it is an array) is being pointed to.

```csharp
// complex type format
FFFF_FFFF_FFFF_FFFF

// instance   member    index
   FFFF_      FFFF_    FFFF_FFFF
```

```csharp
public class MyClass
{
    public int Id {get;set;}
    public float Value {get; set;}
    public int[] Numbers {get;set;} = { 1, 2, 3, 4, 5};

    public int GetValue()
    {
        return Value;
    }
}
```

```csharp
var instance = new MyClass();

// pointer to instance
0001_0000_0000_0000

// pointer to instance.Id
0001_0001_0000_0000

// pointer to instance.Value
0001_0002_0000_0000

// pointer to instance.Numbers[0]
0001_0003_0000_0000

// pointer to instance.Members[4]
0001_0003_0000_0004

// pointer to instance.Members[265373]
0001_0003_0004_0C9D

// pointer to instance.GetValue()
0001_0004_0000_0000
```

## Resource Management Semantics

### Allocation

Pointers should be allocated sequentially starting from `NullPtr/0`. (These are not real pointers pointing to memory).

Pointers can point to previously freed locations.

Any attempts to over-allocate should result in a `NullPtr` and whether that should throw an `NullReferenceException` or have any other behaviour ,other than returning a `NullPtr`, is left as implementation defined.

Allocation of previosly free pointers is implementation defined, and likely to be random or in order in which the pointers were freed.

### Freeing

Pointers should be freed when not in use. Any object that is freed should be made eligible for garbage collection by the CLR on the clien/server for which the object was freed. 

Any freed pointers must be made available for later re-use for re-allocation.

### Referencing Objects using NetPtr

The receiving client/server should handle any variation of a pointer and should never throw an exception or return an exception response.

If a pointer is improperly encoded, or points to a non-existing object a `NullPtr` response should be returned.

