Equ Releases
=============

*Equ uses [SemVer](http://semver.org/) as versioning scheme.*


## 2.3.0

- [PR #24](https://github.com/thedmi/Equ/pull/24) Better integration with C# 9 record types through `EquCompare<T>`.


## 2.2.0

- [PR #21](https://github.com/thedmi/Equ/pull/21) The member resolution logic of Equ can now also be used for ToString() generation.


## 2.1.0

- [PR #19](https://github.com/thedmi/Equ/pull/19) The assembly has now a strong name. Thanks to @gasparnagy for the PR.
- [PR #18](https://github.com/thedmi/Equ/pull/18) Target `netstandard2.0` in addition to the previous `netstandard1.5`.
- [PR #17](https://github.com/thedmi/Equ/pull/17) Indexers are now ignored. The data that indexers expose is typically contained in fields anyway, so the value objects semantics are preserved. Thanks to @ruler501 for the PR.
- [PR #16](https://github.com/thedmi/Equ/pull/16) The equality comparison for dictionaries and sets disregards the order now. Thanks to @ruler501 for the initial implementation.


## 2.0.2

- [PR #6](https://github.com/thedmi/Equ/pull/6) `ElementwiseSequenceEqualityComparer.GetHashCode()` handles now `null` values correctly. Thanks to **zgabi** for the PR.


## 2.0.1

- `[MemberwiseEqualityIgnore]` mechanism fixed for cases where auto properties are used with the field-based equality comparer.


## 2.0.0

- Library re-targeted to .NET Standard 1.5. The functionality itself is unchanged (same as 1.0.4).


## 1.0.4

- Empty objects are now supported as expected. This fixes issue #3.


## 1.0.3

- `PropertywiseEquatable` accidentally compared by field instead of by property (issue #1). This has been fixed.


## 1.0.2


- `PropertywiseEquatable` introduced as convenience base class for property-based equality comparison.
- Value types are now boxed before comparing them. This fixes problems with value types that do not provide an equality function.


## 1.0.1

- Fixes in the Nuget package structure, no library changes.

## 1.0.0

- First production-ready release.
