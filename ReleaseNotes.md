Equ Releases
=============

*Equ uses [SemVer](http://semver.org/) as versioning scheme.*

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
