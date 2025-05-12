import AddIcon from "@mui/icons-material/Add";
import EditIcon from "@mui/icons-material/Edit";
import FilterListIcon from "@mui/icons-material/FilterList";
import {
  Alert,
  Box,
  Button,
  Card,
  CardContent,
  Chip,
  CircularProgress,
  FormControl,
  IconButton,
  InputLabel,
  MenuItem,
  Paper,
  Select,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TablePagination,
  TableRow,
  Typography,
  type LabelDisplayedRowsArgs,
  type SelectChangeEvent,
} from "@mui/material";
import { useEffect, useState } from "react";
import { Link, useSearchParams } from "react-router-dom";
import type Permission from "../data/permission";
import type PermissionType from "../data/permission_type";
import permissions_service from "../services/permissions_service";
import EditPermissionDialog from "./EditPermissionDialog";

// State interfaces
interface DataState {
  permissions: Permission[];
  permissionTypes: PermissionType[];
  hasMore: boolean;
}

interface UIState {
  loading: boolean;
  error: string;
}

interface PaginationState {
  page: number;
  rowsPerPage: number;
}

interface EditState {
  open: boolean;
  loading: boolean;
  error: string;
  permission: {
    id: number;
    employee_name: string;
    employee_surname: string;
    permission_type_id: number;
  };
}

export default function PermissionList() {
  const [searchParams, setSearchParams] = useSearchParams();

  const initialPage = parseInt(searchParams.get("page") || "0");
  const initialRowsPerPage = parseInt(searchParams.get("page_size") || "5");
  const initialPermissionType = searchParams.get("permission_type_id")
    ? parseInt(searchParams.get("permission_type_id") || "0")
    : undefined;

  const [dataState, setDataState] = useState<DataState>({
    permissions: [],
    permissionTypes: [],
    hasMore: false,
  });

  const [uiState, setUIState] = useState<UIState>({
    loading: true,
    error: "",
  });

  const [paginationState, setPaginationState] = useState<PaginationState>({
    page: initialPage,
    rowsPerPage: initialRowsPerPage,
  });

  const [selectedPermissionType, setSelectedPermissionType] = useState<
    number | undefined
  >(initialPermissionType);

  const [editState, setEditState] = useState<EditState>({
    open: false,
    loading: false,
    error: "",
    permission: {
      id: 0,
      employee_name: "",
      employee_surname: "",
      permission_type_id: 0,
    },
  });

  useEffect(() => {
    const params = new URLSearchParams();

    if (paginationState.page !== 0) {
      params.set("page", paginationState.page.toString());
    }

    if (paginationState.rowsPerPage !== 5) {
      params.set("page_size", paginationState.rowsPerPage.toString());
    }

    if (selectedPermissionType !== undefined && selectedPermissionType !== 0) {
      params.set("permission_type_id", selectedPermissionType.toString());
    }

    setSearchParams(params);
  }, [
    paginationState.page,
    paginationState.rowsPerPage,
    selectedPermissionType,
    setSearchParams,
  ]);

  useEffect(() => {
    const fetchData = async () => {
      try {
        setUIState((prev) => ({ ...prev, loading: true }));

        const [permissionsRes, typesRes] = await Promise.all([
          permissions_service.getPermissions({
            pageSize: paginationState.rowsPerPage,
            page: paginationState.page + 1, // because mui its 0 based
            permissionTypeId: selectedPermissionType,
          }),
          permissions_service.getPermissionTypes(),
        ]);

        setDataState({
          permissions: permissionsRes.items,
          permissionTypes: typesRes,
          hasMore: permissionsRes.hasMore,
        });

        setUIState((prev) => ({ ...prev, error: "" }));
      } catch (err) {
        console.error("Error fetching data:", err);
        setUIState((prev) => ({
          ...prev,
          error: "Failed to load data. Please try again later.",
        }));
      } finally {
        setUIState((prev) => ({ ...prev, loading: false }));
      }
    };

    fetchData();
  }, [
    selectedPermissionType,
    paginationState.page,
    paginationState.rowsPerPage,
  ]);

  const handleEdit = (permission: Permission) => {
    setEditState({
      ...editState,
      open: true,
      error: "",
      permission: {
        id: permission.id,
        employee_name: permission.employee_name,
        employee_surname: permission.employee_surname,
        permission_type_id: permission.permission_type.id,
      },
    });
  };

  const handleUpdateEditField = (field: string, value: string | number) => {
    setEditState((prev) => ({
      ...prev,
      permission: {
        ...prev.permission,
        [field]: value,
      },
    }));
  };

  const handleCloseEdit = () => {
    setEditState((prev) => ({ ...prev, open: false }));
  };

  const handleSaveEdit = async () => {
    const { permission } = editState;

    if (
      !permission.employee_name.trim() ||
      !permission.employee_surname.trim()
    ) {
      setEditState((prev) => ({
        ...prev,
        error: "Please fill all required fields.",
      }));
      return;
    }

    try {
      setEditState((prev) => ({
        ...prev,
        loading: true,
        error: "",
      }));

      await permissions_service.modifyPermission({
        permissionId: permission.id,
        employeeName: permission.employee_name,
        employeeSurname: permission.employee_surname,
        permissionTypeId: permission.permission_type_id,
      });

      // Update permissions list with edited data
      const updatedPermissions = dataState.permissions.map((p) =>
        p.id === permission.id
          ? {
              ...p,
              employee_name: permission.employee_name,
              employee_surname: permission.employee_surname,
              permission_type:
                dataState.permissionTypes.find(
                  (t) => t.id === permission.permission_type_id
                ) || p.permission_type,
            }
          : p
      );

      setDataState((prev) => ({
        ...prev,
        permissions: updatedPermissions,
      }));

      setEditState((prev) => ({
        ...prev,
        open: false,
      }));
    } catch (err) {
      console.error("Error updating permission:", err);
      setEditState((prev) => ({
        ...prev,
        error: "Failed to update permission. Please try again.",
      }));
    } finally {
      setEditState((prev) => ({
        ...prev,
        loading: false,
      }));
    }
  };

  // Pagination handlers
  const handleChangePage = (_event: unknown, newPage: number) => {
    setPaginationState((prev) => ({
      ...prev,
      page: newPage,
    }));
  };

  const handleChangeRowsPerPage = (
    event: React.ChangeEvent<HTMLInputElement>
  ) => {
    setPaginationState({
      page: 0,
      rowsPerPage: parseInt(event.target.value, 10),
    });
  };

  // Filter handler
  const handleFilterChange = (event: SelectChangeEvent<number>) => {
    const newValue = event.target.value as number;
    setSelectedPermissionType(newValue);

    // reset page when changing filters
    setPaginationState((prev) => ({
      ...prev,
      page: 0,
    }));
  };

  function labelDisplayedRows({ from, to }: LabelDisplayedRowsArgs) {
    return `${from}-${to}`;
  }

  const { permissions, permissionTypes, hasMore } = dataState;
  const { loading, error } = uiState;
  const { page, rowsPerPage } = paginationState;

  return (
    <Card elevation={3} sx={{ width: "100%" }}>
      <CardContent sx={{ p: 3 }}>
        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
            mb: 3,
          }}
        >
          <Typography variant="h5" component="h1" fontWeight="bold">
            Permissions List
          </Typography>
          <Button
            variant="contained"
            color="primary"
            component={Link}
            to="/new"
            startIcon={<AddIcon />}
          >
            New Permission
          </Button>
        </Box>

        {error && (
          <Alert severity="error" sx={{ mb: 3 }}>
            {error}
          </Alert>
        )}

        {/* Filter controls */}
        <Box sx={{ mb: 3, display: "flex", alignItems: "center" }}>
          <FormControl variant="outlined" sx={{ minWidth: 200 }}>
            <InputLabel id="permission-type-filter-label">
              Filter by Type
            </InputLabel>
            <Select
              labelId="permission-type-filter-label"
              value={selectedPermissionType}
              onChange={handleFilterChange}
              label="Filter by Type"
              startAdornment={<FilterListIcon color="action" sx={{ mr: 1 }} />}
            >
              <MenuItem value={0}>All Types</MenuItem>
              {permissionTypes.map((type) => (
                <MenuItem key={type.id} value={type.id}>
                  {type.description}
                </MenuItem>
              ))}
            </Select>
          </FormControl>
          {selectedPermissionType !== 0 && (
            <Button
              sx={{ ml: 1 }}
              variant="outlined"
              size="small"
              onClick={() => setSelectedPermissionType(0)}
            >
              Clear Filter
            </Button>
          )}
        </Box>

        {loading ? (
          <Box sx={{ display: "flex", justifyContent: "center", p: 3 }}>
            <CircularProgress />
          </Box>
        ) : permissions.length > 0 ? (
          <>
            <TableContainer component={Paper} variant="outlined">
              <Table>
                <TableHead>
                  <TableRow>
                    <TableCell>Employee</TableCell>
                    <TableCell>Permission Type</TableCell>
                    <TableCell align="right">Date</TableCell>
                    <TableCell align="center">Actions</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {permissions.map((p) => (
                    <TableRow key={p.id} hover>
                      <TableCell>
                        <Box>
                          <Typography
                            variant="subtitle2"
                            sx={{ fontWeight: "medium" }}
                          >
                            {p.employee_name} {p.employee_surname}
                          </Typography>
                        </Box>
                      </TableCell>
                      <TableCell>
                        <Chip
                          label={p.permission_type.description}
                          size="small"
                          color="primary"
                          variant="outlined"
                        />
                      </TableCell>
                      <TableCell align="right">
                        {new Date(p.granted_at).toLocaleDateString()}
                      </TableCell>
                      <TableCell align="center">
                        <IconButton
                          size="small"
                          color="primary"
                          onClick={() => handleEdit(p)}
                          aria-label="edit"
                        >
                          <EditIcon fontSize="small" />
                        </IconButton>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
            <TablePagination
              rowsPerPageOptions={[5, 10, 25, 50, 100]}
              component="div"
              count={-1}
              slotProps={{
                actions: {
                  nextButton: {
                    disabled: !hasMore,
                  },
                },
              }}
              labelDisplayedRows={labelDisplayedRows}
              rowsPerPage={rowsPerPage}
              page={page}
              onPageChange={handleChangePage}
              onRowsPerPageChange={handleChangeRowsPerPage}
            />
          </>
        ) : (
          <Alert severity="info">
            {permissions.length === 0
              ? "No permissions found. Create a new one!"
              : "No permissions match the selected filter."}
          </Alert>
        )}
      </CardContent>

      <EditPermissionDialog
        open={editState.open}
        permission={editState.permission}
        permissionTypes={permissionTypes}
        loading={editState.loading}
        error={editState.error}
        onClose={handleCloseEdit}
        onSave={handleSaveEdit}
        onUpdateField={handleUpdateEditField}
      />
    </Card>
  );
}
