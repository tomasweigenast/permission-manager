import CloseIcon from "@mui/icons-material/Close";
import SaveIcon from "@mui/icons-material/Save";
import {
  Alert,
  Box,
  Button,
  CircularProgress,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  IconButton,
  MenuItem,
  TextField,
} from "@mui/material";
import type PermissionType from "../data/permission_type";

interface EditPermissionDialogProps {
  open: boolean;
  permission: {
    id: number;
    employee_name: string;
    employee_surname: string;
    permission_type_id: number;
  };
  permissionTypes: PermissionType[];
  loading: boolean;
  error: string;
  onClose: () => void;
  onSave: (updatedPermission: {
    permissionId: number;
    employeeName: string;
    employeeSurname: string;
    permissionTypeId: number;
  }) => void;
  onUpdateField: (field: string, value: string | number) => void;
}

export default function EditPermissionDialog({
  open,
  permission,
  permissionTypes,
  loading,
  error,
  onClose,
  onSave,
  onUpdateField,
}: EditPermissionDialogProps) {
  const handleSave = () => {
    onSave({
      permissionId: permission.id,
      employeeName: permission.employee_name,
      employeeSurname: permission.employee_surname,
      permissionTypeId: permission.permission_type_id,
    });
  };

  return (
    <Dialog open={open} onClose={onClose} maxWidth="sm" fullWidth>
      <DialogTitle>
        <Box
          sx={{
            display: "flex",
            justifyContent: "space-between",
            alignItems: "center",
          }}
        >
          Edit Permission
          <IconButton onClick={onClose} size="small">
            <CloseIcon />
          </IconButton>
        </Box>
      </DialogTitle>
      <DialogContent>
        {error && (
          <Alert severity="error" sx={{ mb: 3, mt: 1 }}>
            {error}
          </Alert>
        )}
        <Box sx={{ pt: 1, display: "flex", flexDirection: "column", gap: 2 }}>
          <TextField
            fullWidth
            label="First Name"
            value={permission.employee_name}
            onChange={(e) => onUpdateField("employee_name", e.target.value)}
            variant="outlined"
            required
            InputLabelProps={{ shrink: true }}
            placeholder="Enter employee's first name"
          />
          <TextField
            fullWidth
            label="Last Name"
            value={permission.employee_surname}
            onChange={(e) => onUpdateField("employee_surname", e.target.value)}
            variant="outlined"
            required
            InputLabelProps={{ shrink: true }}
            placeholder="Enter employee's last name"
          />
          <TextField
            fullWidth
            select
            label="Permission Type"
            value={permission.permission_type_id}
            onChange={(e) =>
              onUpdateField("permission_type_id", Number(e.target.value))
            }
            variant="outlined"
            required
            InputLabelProps={{ shrink: true }}
            disabled={permissionTypes.length === 0}
          >
            {permissionTypes.map((t) => (
              <MenuItem key={t.id} value={t.id}>
                {t.description}
              </MenuItem>
            ))}
          </TextField>
        </Box>
      </DialogContent>
      <DialogActions sx={{ px: 3, pb: 2 }}>
        <Button onClick={onClose} color="inherit" variant="outlined">
          Cancel
        </Button>
        <Button
          onClick={handleSave}
          color="primary"
          variant="contained"
          startIcon={loading ? <CircularProgress size={20} /> : <SaveIcon />}
          disabled={loading}
        >
          {loading ? "Saving..." : "Save Changes"}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
