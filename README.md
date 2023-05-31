# PanoramicData.SecurityChecker
[![Nuget](https://img.shields.io/nuget/v/PanoramicData.SecurityChecker)](https://www.nuget.org/packages/PanoramicData.SecurityChecker/)
[![Nuget](https://img.shields.io/nuget/dt/PanoramicData.SecurityChecker)](https://www.nuget.org/packages/PanoramicData.SecurityChecker/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/fe2bc15fbeac4baab12e55729edeb652)](https://app.codacy.com/gh/panoramicdata/PanoramicData.SecurityChecker/dashboard?utm_source=gh&utm_medium=referral&utm_content=&utm_campaign=Badge_grade)

Runtime security checking for .NET applications.

Functionality is split into two separate packages.

The core functionality resides within the PanoramicData.SecurityChecker package.
Features to enable exposing the core functionality via an endpoint resides within the PanoramicData.SecurityChecker.AspNetCore package.

The repo also contains a demo project; this project demonstrates how the packages can be used to expose security checks to an external monitoring system. 
Note that the basic implementation does not offer any authentication or throttling at this moment in time. We **highly** recommend using both to help protect your system.
Security through obscurity is a known anti-pattern, but at the same time the reporting of potential security vulnerabilities to unauthenticated users is not 
a wise approach either.