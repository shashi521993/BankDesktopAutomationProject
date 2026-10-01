# Test: BankManagementSystem.ViewAccountsWindow

Source: BankManagementSystem/ViewAccountsWindow.xaml

## Purpose
Validate primary interactions on the BankManagementSystem.ViewAccountsWindow window (display, inputs, primary commands).

## Preconditions
- Application installed or built locally
- Test user data seeded if required (see testData)

## Test data
- sampleUserId: testuser
- samplePassword: Password123

## Steps
1. Launch the application.
2. Navigate/open the BankManagementSystem.ViewAccountsWindow window (if not the startup window) — use application menu or command sequence.
3. Enter sample data into control 'dgAccounts'.
4. Click control that triggers handler 'BtnDelete_Click' (defined in: BankManagementSystem/ViewAccountsWindow.xaml.cs).
5. Observe application behavior and final state.

## Expected Results
- Window displays without error and all controls visible.
- No unhandled exception dialogs appear.
- For primary actions, application transitions to the expected screen or updates persisted state as specified in traceability.

## Traceability
- XAML: BankManagementSystem/ViewAccountsWindow.xaml
- Handler: BtnDelete_Click -> BankManagementSystem/ViewAccountsWindow.xaml.cs

## Notes/Flags
- Automatically generated test. Review steps and data for business correctness and add DB seeds if required.
