# Test: BankManagementSystem.DepositWindow

Source: BankManagementSystem/DepositWindow.xaml

## Purpose
Validate primary interactions on the BankManagementSystem.DepositWindow window (display, inputs, primary commands).

## Preconditions
- Application installed or built locally
- Test user data seeded if required (see testData)

## Test data
- sampleUserId: testuser
- samplePassword: Password123

## Steps
1. Launch the application.
2. Navigate/open the BankManagementSystem.DepositWindow window (if not the startup window) — use application menu or command sequence.
3. Enter sample data into control 'txtAccountNumber'.
4. Enter sample data into control 'txtAmount'.
5. Click control that triggers handler 'BtnDeposit_Click' (defined in: BankManagementSystem/DepositWindow.xaml.cs).
6. Observe application behavior and final state.

## Expected Results
- Window displays without error and all controls visible.
- No unhandled exception dialogs appear.
- For primary actions, application transitions to the expected screen or updates persisted state as specified in traceability.

## Traceability
- XAML: BankManagementSystem/DepositWindow.xaml
- Handler: BtnDeposit_Click -> BankManagementSystem/DepositWindow.xaml.cs

## Notes/Flags
- Automatically generated test. Review steps and data for business correctness and add DB seeds if required.
