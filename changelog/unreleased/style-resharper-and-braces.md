type: internal

Source-only cleanup: the per-member "ReSharper disable once" comments on public members are removed and the remaining single-line blocks use braces; no API or behaviour change, except that the obsolete `To(address, displayName)` now treats an empty display name like `From` does.
