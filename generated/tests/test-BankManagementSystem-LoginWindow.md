# Test: BankManagementSystem.LoginWindow

Source: BankManagementSystem/LoginWindow.xaml

## Purpose
Validate primary interactions on the BankManagementSystem.LoginWindow window (display, inputs, primary commands).

## Preconditions
- Application installed or built locally
- Test user data seeded if required (see testData)

## Test data
- sampleUserId: testuser
- samplePassword: Password123

## Steps
1. Launch the application.
2. Navigate/open the BankManagementSystem.LoginWindow window (if not the startup window) — use application menu or command sequence.
3. Enter sample data into control 'txtUserId'.
4. Enter sample data into control 'txtPassword'.
5. Click control that triggers handler 'BtnLogin_Click' (defined in: BankManagementSystem/LoginWindow.xaml.cs).
6. Click control that triggers handler 'BtnClear_Click' (defined in: BankManagementSystem/LoginWindow.xaml.cs).
7. Observe application behavior and final state.

## Expected Results
- Window displays without error and all controls visible.
- No unhandled exception dialogs appear.
- For primary actions, application transitions to the expected screen or updates persisted state as specified in traceability.

## Traceability
- XAML: BankManagementSystem/LoginWindow.xaml
- Handler: BtnLogin_Click -> BankManagementSystem/LoginWindow.xaml.cs
- Handler: BtnClear_Click -> BankManagementSystem/LoginWindow.xaml.cs

## Notes/Flags
- Automatically generated test. Review steps and data for business correctness and add DB seeds if required.
