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
import { Link } from "react-router-dom";
import type Permission from "../data/permission";
import type PermissionType from "../data/permission_type";
import permissions_service from "../services/permissions_service";
import EditPermissionDialog from "./EditPermissionDialog";

export default function PermissionList() {
  const [permissions, setPermissions] = useState<Permission[]>([]);
  const [permissionTypes, setPermissionTypes] = useState<PermissionType[]>([]);
  const [hasMore, setHasMore] = useState(false);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [editDialogOpen, setEditDialogOpen] = useState(false);
  const [editLoading, setEditLoading] = useState(false);
  const [editError, setEditError] = useState("");

  // Pagination state
  const [page, setPage] = useState(0);
  const [rowsPerPage, setRowsPerPage] = useState(5);

  // Filter state
  const [selectedPermissionType, setSelectedPermissionType] = useState<
    number | undefined
  >(undefined);

  // State for edited permission
  const [editedPermission, setEditedPermission] = useState<{
    id: number;
    employee_name: string;
    employee_surname: string;
    permission_type_id: number;
  }>({
    id: 0,
    employee_name: "",
    employee_surname: "",
    permission_type_id: 0,
  });

  useEffect(() => {
    const fetchData = async () => {
      try {
        setLoading(true);
        const [permissionsRes, typesRes] = await Promise.all([
          permissions_service.getPermissions({
            pageSize: rowsPerPage,
            page: page + 1, // because mui its 0 based
            permissionTypeId: selectedPermissionType,
          }),
          permissions_service.getPermissionTypes(),
        ]);
        setPermissions(permissionsRes.items);
        setPermissionTypes(typesRes);
        setHasMore(permissionsRes.hasMore);
        setError("");
      } catch (err) {
        console.error("Error fetching data:", err);
        setError("Failed to load data. Please try again later.");
      } finally {
        setLoading(false);
      }
    };

    fetchData();
  }, [selectedPermissionType, page, rowsPerPage]);

  const handleEdit = (permission: Permission) => {
    setEditedPermission({
      id: permission.id,
      employee_name: permission.employee_name,
      employee_surname: permission.employee_surname,
      permission_type_id: permission.permission_type.id,
    });
    setEditDialogOpen(true);
    setEditError("");
  };

  const handleUpdateEditField = (field: string, value: string | number) => {
    setEditedPermission((prev) => ({
      ...prev,
      [field]: value,
    }));
  };

  const handleCloseEdit = () => {
    setEditDialogOpen(false);
  };

  const handleSaveEdit = async () => {
    if (
      !editedPermission.employee_name.trim() ||
      !editedPermission.employee_surname.trim()
    ) {
      setEditError("Please fill all required fields.");
      return;
    }

    try {
      setEditLoading(true);
      setEditError("");

      await permissions_service.modifyPermission({
        permissionId: editedPermission.id,
        employeeName: editedPermission.employee_name,
        employeeSurname: editedPermission.employee_surname,
        permissionTypeId: editedPermission.permission_type_id,
      });

      // Update permissions list with edited data
      const updatedPermissions = permissions.map((p) =>
        p.id === editedPermission.id
          ? {
              ...p,
              employee_name: editedPermission.employee_name,
              employee_surname: editedPermission.employee_surname,
              permission_type:
                permissionTypes.find(
                  (t) => t.id === editedPermission.permission_type_id
                ) || p.permission_type,
            }
          : p
      );

      setPermissions(updatedPermissions);
      setEditDialogOpen(false);
    } catch (err) {
      console.error("Error updating permission:", err);
      setEditError("Failed to update permission. Please try again.");
    } finally {
      setEditLoading(false);
    }
  };

  // Pagination handlers
  const handleChangePage = (_event: unknown, newPage: number) => {
    setPage(newPage);
  };

  const handleChangeRowsPerPage = (
    event: React.ChangeEvent<HTMLInputElement>
  ) => {
    setRowsPerPage(parseInt(event.target.value, 10));
    setPage(0);
  };

  // Filter handler
  const handleFilterChange = (event: SelectChangeEvent<number>) => {
    setSelectedPermissionType(event.target.value);
  };

  function labelDisplayedRows({ from, to }: LabelDisplayedRowsArgs) {
    return `${from}-${to}`;
  }

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
        open={editDialogOpen}
        permission={editedPermission}
        permissionTypes={permissionTypes}
        loading={editLoading}
        error={editError}
        onClose={handleCloseEdit}
        onSave={handleSaveEdit}
        onUpdateField={handleUpdateEditField}
      />
    </Card>
  );
}
