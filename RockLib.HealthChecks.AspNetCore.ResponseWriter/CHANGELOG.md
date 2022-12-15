# RockLib.HealthChecks.AspNetCore.ResponseWriter Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## 2.0.0-alpha.1 - 2022-12-15
	
#### Added
- Added `.editorconfig` and `Directory.Build.props` files to ensure consistency.

#### Changed
- Supported targets: net6.0, netcoreapp3.1, and net48.
- As the package now uses nullable reference types, some method parameters now specify if they can accept nullable values.

## 1.1.2 - 2021-08-13

#### Changed

- Changes "Quicken Loans" to "Rocket Mortgage".
- Updates RockLib.HealthChecks to latest version, [1.3.9](https://github.com/RockLib/RockLib.HealthChecks/blob/main/RockLib.HealthChecks/CHANGELOG.md#139---2021-08-13).

## 1.1.1 - 2021-05-10

#### Added

- Adds SourceLink to nuget package.

#### Changed

- Updates RockLib.HealthChecks package to latest version, which includes SourceLink.

----

**Note:** Release notes in the above format are not available for earlier versions of
RockLib.HealthChecks.AspNetCore.ResponseWriter. What follows below are the original release notes.

----

## 1.1.0

Adds the ability to hide the exception or description/output when mapping a Microsoft HealthReportEntry to a RockLib HealthCheckResult.

## 1.0.4

Updates RockLib.HealthChecks dependency, adding missing functionality for the net5.0 target.

## 1.0.3

Adds net5.0 target and updates dependencies.

## 1.0.2

Adds icon to project and nuget package.

## 1.0.1

Updates to align with nuget conventions.

## 1.0.0

Initial release.

## 1.0.0-rc1

Initial release candidate.
