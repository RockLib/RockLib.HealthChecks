# RockLib.HealthChecks Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Unreleased
	
#### Added
- Added `.editorconfig` and `Directory.Build.props` files to ensure consistency.

#### Changed
- Supported targets: net6.0, netcoreapp3.1, and net48. This means code in `RockLib.HealthChecks.Configuration` has been removed.
- As the package now uses nullable reference types, some method parameters now specify if they can accept nullable values.

## 1.4.0 - 2021-10-26

#### Changed

- Switches underscore and dotted config section names.

## 1.3.9 - 2021-08-13

#### Changed

- Changes "Quicken Loans" to "Rocket Mortgage".
- Updates RockLib.Configuration to latest version, [2.5.3](https://github.com/RockLib/RockLib.Configuration/blob/main/RockLib.Configuration/CHANGELOG.md#253---2021-08-11).
- Updates RockLib.Configuration.ObjectFactory to latest version, [1.6.9](https://github.com/RockLib/RockLib.Configuration/blob/main/RockLib.Configuration.ObjectFactory/CHANGELOG.md#169---2021-08-11).
- Updates RockLib.Immutable to latest version, [1.0.7](https://github.com/RockLib/RockLib.Immutable/blob/main/RockLib.Immutable/CHANGELOG.md#107---2021-08-10).

## 1.3.8 - 2021-05-10

#### Added

- Adds SourceLink to nuget package.

#### Changed

- Updates RockLib.Configuration, RockLib.Configuration.ObjectFactory, and RockLib.Immutable packages to latest versions, which include SourceLink.
- Updates Newtonsoft.Json package to latest version.

----

**Note:** Release notes in the above format are not available for earlier versions of
RockLib.HealthChecks. What follows below are the original release notes.

----

## 1.3.7

Adds missing functionality for NET5.0 target.

## 1.3.6

Adds net5.0 target and updates RockLib dependencies.

## 1.3.5

Adds net5.0 target.

## 1.3.4

Fixes bug in DiskDriveHealthCheck, protecting against drives that aren't ready.

## 1.3.3

Updates RockLib.Configuration.ObjectFactory for a bug fix.

## 1.3.2

Adds icon to project and nuget package.

## 1.3.1

Updates to align with nuget conventions.

## 1.3.0

Adds an UncaughtExceptionStatus setting to HealthCheckRunner. This value is used as the status of the result that is returned when a health check throws an exception.

## 1.2.0

Adds the ability to specify service lifetime when registering a health check runner.

## 1.1.0

Add idiomatic support for dependency injection.

## 1.0.0

Initial release.

## 1.0.0-rc1

Initial release candidate.
