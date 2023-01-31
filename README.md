Art is a set of packages for streamlining targeted
archival of artifacts such as web articles with
associated resource streams.

Art is intended to be used in plugin architectures,
where implementors of sub-interfaces of
`Art.IArtifactTool` expose capabilities such as
listing, finding, and dumping resources based
on a configuration provided to the tool.

Interfaces for artifact registration management
(e.g. Sqlite) and data management (e.g. a folder on
disk) are supplied. Instances of these interfaces
are to be supplied to tools, primarily for dump
tools, and can also be used to archive retrieved
artifacts to arbitrary backing stores.
