# Test: BankManagementSystem.DashboardWindow

Source: BankManagementSystem/DashboardWindow.xaml

## Purpose
Validate primary interactions on the BankManagementSystem.DashboardWindow window (display, inputs, primary commands).

## Preconditions
- Application installed or built locally
- Test user data seeded if required (see testData)

## Test data
- sampleUserId: testuser
- samplePassword: Password123

## Steps
1. Launch the application.
2. Navigate/open the BankManagementSystem.DashboardWindow window (if not the startup window) — use application menu or command sequence.
3. Enter sample data into control 'BtnCreateAccount'.
4. Enter sample data into control 'BtnDeposit'.
5. Enter sample data into control 'BtnWithdraw'.
6. Enter sample data into control 'BtnCheckBalance'.
7. Enter sample data into control 'BtnModifyAccount'.
8. Enter sample data into control 'BtnViewAccounts'.
9. Enter sample data into control 'BtnLogout'.
10. Click control that triggers handler 'BtnCreateAccount_Click' (defined in: BankManagementSystem/DashboardWindow.xaml.cs).
11. Click control that triggers handler 'BtnDeposit_Click' (defined in: BankManagementSystem/DashboardWindow.xaml.cs).
12. Click control that triggers handler 'BtnWithdraw_Click' (defined in: BankManagementSystem/DashboardWindow.xaml.cs).
13. Click control that triggers handler 'BtnCheckBalance_Click' (defined in: BankManagementSystem/DashboardWindow.xaml.cs).
14. Click control that triggers handler 'BtnModifyAccount_Click' (defined in: BankManagementSystem/DashboardWindow.xaml.cs).
15. Click control that triggers handler 'BtnViewAccounts_Click' (defined in: BankManagementSystem/DashboardWindow.xaml.cs).
16. Click control that triggers handler 'BtnLogout_Click' (defined in: BankManagementSystem/DashboardWindow.xaml.cs).
17. Observe application behavior and final state.

## Expected Results
- Window displays without error and all controls visible.
- No unhandled exception dialogs appear.
- For primary actions, application transitions to the expected screen or updates persisted state as specified in traceability.

## Traceability
- XAML: BankManagementSystem/DashboardWindow.xaml
- Handler: BtnCreateAccount_Click -> BankManagementSystem/DashboardWindow.xaml.cs
- Handler: BtnDeposit_Click -> BankManagementSystem/DashboardWindow.xaml.cs
- Handler: BtnWithdraw_Click -> BankManagementSystem/DashboardWindow.xaml.cs
- Handler: BtnCheckBalance_Click -> BankManagementSystem/DashboardWindow.xaml.cs
- Handler: BtnModifyAccount_Click -> BankManagementSystem/DashboardWindow.xaml.cs
- Handler: BtnViewAccounts_Click -> BankManagementSystem/DashboardWindow.xaml.cs
- Handler: BtnLogout_Click -> BankManagementSystem/DashboardWindow.xaml.cs

## Notes/Flags
- Automatically generated test. Review steps and data for business correctness and add DB seeds if required.
