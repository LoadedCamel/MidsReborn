# Mids Reborn

Mids Reborn is a hero build planning application for *City of Heroes*. It provides a structured environment for designing, modifying, and analyzing character builds while accurately modeling underlying system mechanics and interactions.

The platform is actively maintained and used by a large community, with continuous updates focused on improving system correctness, performance, and usability.

---

## Overview

Mids Reborn allows users to explore and optimize character builds through a system-driven approach.

It enables:

- Creating and modifying character builds
- Exploring interactions between powers, enhancements, and effects
- Analyzing how system rules impact outcomes
- Sharing and importing builds through external integrations

The application is designed to make complex system behavior understandable and accessible.

---

## Key Features

- Full-featured interface for building and editing character configurations
- Accurate modeling of system rules and interactions
- Structured representation of powers, attributes, and enhancements
- Integration with APIs for build sharing and retrieval
- Continuous updates based on user feedback and evolving system data

---

## Community

Official Discord Server:

<https://discord.gg/mids-reborn-593336669004890113>

---

## System Design

Mids Reborn is more than a client application — it is a structured system designed to model complex interactions within a well-defined environment.

### Logic Evaluation Engine

- Models interactions between powers, enhancements, and system rules
- Ensures predictable and consistent behavior across configurations
- Handles layered dependencies and interaction chains

### Data Modeling & Pipelines

- Uses structured datasets to represent system components and attributes
- Transforms source data into formats suitable for application use
- Supports ongoing updates as underlying data evolves

### API Integration

- Enables sharing and retrieval of builds across systems
- Provides structured data access for external applications
- Decouples data services from client functionality

### Application Architecture

- Designed for maintainability and extensibility
- Supports iterative improvements based on real-world usage
- Balances performance, correctness, and usability

---

## Legacy System & Evolution

Mids Reborn is built on top of the original *Mids: Hero Designer*, which was initially developed in Visual Basic around 2008.

Between 2010 and 2012, portions of the system were transitioned to C#, and in 2019 the application was fully ported to C#.

As a result, the project contains a significant legacy codebase that continues to be actively maintained and improved.

---

## Version Evolution

### 3.x — Legacy Stabilization

The 3.x versions were focused on keeping the legacy system stable and functional while introducing targeted improvements where practical.

This phase included:

- Maintaining compatibility with the existing legacy architecture
- Keeping older systems operational as the application evolved
- Refactoring selected legacy components
- Improving and adding classes where needed
- Introducing new features within the limits of the existing architecture
- Addressing stability, usability, and maintenance concerns over time

The 3.x line served as an important transition period between the original legacy codebase and the broader modernization work that followed.

---

## Current Approach

### 4.x — Windows Forms Modernization & Correction Phase

The 4.x versions represent the current active development approach for Mids Reborn.

This phase focuses on modernizing the Windows Forms application while also addressing the limitations, bugs, and structural issues carried forward from earlier versions.

Key areas of focus includes:
- Identifying and correcting legacy bugs and system inconsistencies from 3.x
- Incremental modernization of legacy systems
- Refactoring older components for maintainability and clarity
- Improving application architecture where practical
- Updating system behavior for correctness and reliability
- Improving performance and long-term maintainability
- Adding new features while preserving compatibility
- Maintaining a stable Windows Forms-based application

The goal of 4.x is to improve the reliability and quality of the existing system while continuing to modernize the codebase and prepare for furute architectural changes.

---

## Planned Architecture

### 5.x — Architectural & Platform Modernization

The 5.x version is planned as a major architectural and platform evolution of Mids Reborn.

While the 4.x line focuses on improving and stabilizing the existing Windows Forms application, the 5.x line introduces a modern architecture along with cross-platform capabilities.

Key goals include:
- Transitioning from Windows Forms to **Avalonia UI** to enable cross-platform support (Windows, MacOS, Linux)
- Clear separation between user interface, application logic, data models, and calculation systems
- Improved data modeling for powers, enhancements, attributes, and build state
- A modular calculation engine designed for correctness, testability, and future expansion
- Cleaner abstraction around data loading, validation, and transformation
- Reduced coupling between UI and core system logic
- Improved support for automated testing and validation
- Greater flexibility for future platforms and interface changes

The goal of 5.x is to move beyond legacy constraints and establish a more maintainable, scalable, and cross-platform system for long-term development.

---

## Development Approach

The avolution of Mids Reborn follos a staged, real-world system modernization strategy:
- **3.x** focused on stabilizing and extending the legacy system while preserving compatibility
- **4.x** represents the active modernization phase, where legacy flaws are addressed and system components are incrementally refactored and improved
- **5.x** introduces both architectural and platform-level transformation, moving to a modular design while enabling cross-platform support

This phased approach allows the project to balance stability, continuity, and user expectations while progressively reducing technical debt and improving system design.

Rather than replacing the system entirely at once, Mids Reborn evolves through controlled, incremental changes that maintain usability while preparing for larger architectural and platform improvements.

---

## Development & Ownership

This project is independently owned and maintained.

Responsibilities include:

- System architecture design and evolution
- Feature development and implementation
- Data pipeline development and maintenance
- API integration and system interaction
- Continuous refinement based on user feedback and usage patterns

---

## Goals

The primary goals of Mids Reborn are:

- Provide accurate modeling of complex system interactions
- Maintain correctness as underlying data evolves
- Improve user understanding of system behavior
- Enable exploration and optimization through structured workflows

---

## Requirements

### Runtime Requirements

#### Windows

Mids Reborn currently requires Windows.

Supported Windows versions:

- Windows 10 version 2004, also known as the May 2020 Update, or higher
- Windows 11 is recommended

Additional requirements:

- [.NET 8 Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

Versions below Windows 10 version 2004 are unlikely to work due to .NET 8 requirements.

#### macOS & Linux

macOS and Linux are not officially supported.

The application may work using a 64-bit Wine prefix, but this is not recommended for general use.

---

## Distribution & Updates

Windows setup binaries are generated using Advanced Installer.

Mids Reborn uses a customized Bootstrap Updater, developed by Metalios and licensed to LoadedCamel.

---

## Development

### Building from Source

#### Prerequisites

To build Mids Reborn from source, you will need:

- Visual Studio 2022
- .NET 8 SDK

#### Build Steps

Clone the repository:

```bash
git clone https://github.com/LoadedCamel/MidsReborn.git
```

Then:

1. Open the solution file in Visual Studio 2022.
2. Restore NuGet packages.
   - This usually happens automatically when the solution loads.
3. Build the solution.
4. Run the application.

### Notes

- This repository contains the full source code.
- Source builds are intended for development and contribution, not end-user runtime installation.

---

## Contributions

Contributions, feedback, and issue reports are welcome and help guide ongoing improvements to the platform.

---

## Credits

Thanks to everyone who has contributed to Mids Reborn.

[View repository contributors](https://github.com/LoadedCamel/MidsReborn/graphs/contributors)

---

## Summary

Mids Reborn is a long-running, actively maintained platform designed to model complex system behavior and allow users to explore, analyze, and share structured configurations.

The focus of the project is on correctness, usability, and continuous system improvement driven by real-world usage.
