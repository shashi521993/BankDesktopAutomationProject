# Test: BankManagementSystem.CreateAccountWindow

Source: BankManagementSystem/CreateAccountWindow.xaml

## Purpose
Validate primary interactions on the BankManagementSystem.CreateAccountWindow window (display, inputs, primary commands).

## Preconditions
- Application installed or built locally
- Test user data seeded if required (see testData)

## Test data
- sampleUserId: testuser
- samplePassword: Password123

## Steps
1. Launch the application.
2. Navigate/open the BankManagementSystem.CreateAccountWindow window (if not the startup window) — use application menu or command sequence.
3. Enter sample data into control 'cmbAccountType'.
4. Enter sample data into control 'txtFirstName'.
5. Enter sample data into control 'txtMiddleName'.
6. Enter sample data into control 'txtLastName'.
7. Enter sample data into control 'dpDOB'.
8. Enter sample data into control 'cmbGender'.
9. Enter sample data into control 'txtPan'.
10. Enter sample data into control 'txtCurrentAddress'.
11. Enter sample data into control 'txtPermanentAddress'.
12. Enter sample data into control 'txtMobile'.
13. Click control that triggers handler 'BtnSave_Click' (defined in: BankManagementSystem/CreateAccountWindow.xaml.cs).
14. Click control that triggers handler 'BtnClear_Click' (defined in: BankManagementSystem/CreateAccountWindow.xaml.cs).
15. Observe application behavior and final state.

## Expected Results
- Window displays without error and all controls visible.
- No unhandled exception dialogs appear.
- For primary actions, application transitions to the expected screen or updates persisted state as specified in traceability.

## Traceability
- XAML: BankManagementSystem/CreateAccountWindow.xaml
- Handler: BtnSave_Click -> BankManagementSystem/CreateAccountWindow.xaml.cs
- Handler: BtnClear_Click -> BankManagementSystem/CreateAccountWindow.xaml.cs

## Notes/Flags
- Automatically generated test. Review steps and data for business correctness and add DB seeds if required.
