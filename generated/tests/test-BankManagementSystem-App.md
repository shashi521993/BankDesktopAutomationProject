# Test: BankManagementSystem.App

Source: BankManagementSystem/App.xaml

## Purpose
Validate primary interactions on the BankManagementSystem.App window (display, inputs, primary commands).

## Preconditions
- Application installed or built locally
- Test user data seeded if required (see testData)

## Test data
- sampleUserId: testuser
- samplePassword: Password123

## Steps
1. Launch the application.
2. Navigate/open the BankManagementSystem.App window (if not the startup window) — use application menu or command sequence.
3. Observe application behavior and final state.

## Expected Results
- Window displays without error and all controls visible.
- No unhandled exception dialogs appear.
- For primary actions, application transitions to the expected screen or updates persisted state as specified in traceability.

## Traceability
- XAML: BankManagementSystem/App.xaml

## Notes/Flags
- Automatically generated test. Review steps and data for business correctness and add DB seeds if required.
