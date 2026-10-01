# Test: BankManagementSystem.ModifyAccountWindow

Source: BankManagementSystem/ModifyAccountWindow.xaml

## Purpose
Validate primary interactions on the BankManagementSystem.ModifyAccountWindow window (display, inputs, primary commands).

## Preconditions
- Application installed or built locally
- Test user data seeded if required (see testData)

## Test data
- sampleUserId: testuser
- samplePassword: Password123

## Steps
1. Launch the application.
2. Navigate/open the BankManagementSystem.ModifyAccountWindow window (if not the startup window) — use application menu or command sequence.
3. Enter sample data into control 'txtAccountNumber'.
4. Enter sample data into control 'txtFirstName'.
5. Enter sample data into control 'txtMiddleName'.
6. Enter sample data into control 'txtLastName'.
7. Enter sample data into control 'txtMobile'.
8. Enter sample data into control 'txtCurrentAddress'.
9. Enter sample data into control 'txtPermanentAddress'.
10. Click control that triggers handler 'BtnSearch_Click' (defined in: BankManagementSystem/ModifyAccountWindow.xaml.cs).
11. Click control that triggers handler 'BtnUpdate_Click' (defined in: BankManagementSystem/ModifyAccountWindow.xaml.cs).
12. Observe application behavior and final state.

## Expected Results
- Window displays without error and all controls visible.
- No unhandled exception dialogs appear.
- For primary actions, application transitions to the expected screen or updates persisted state as specified in traceability.

## Traceability
- XAML: BankManagementSystem/ModifyAccountWindow.xaml
- Handler: BtnSearch_Click -> BankManagementSystem/ModifyAccountWindow.xaml.cs
- Handler: BtnUpdate_Click -> BankManagementSystem/ModifyAccountWindow.xaml.cs

## Notes/Flags
- Automatically generated test. Review steps and data for business correctness and add DB seeds if required.
