import ArrowBackIcon from "@mui/icons-material/ArrowBack";
import SaveIcon from "@mui/icons-material/Save";
import {
  Alert,
  Box,
  Button,
  Card,
  CardContent,
  CircularProgress,
  Container,
  Divider,
  Grid,
  IconButton,
  MenuItem,
  TextField,
  Typography,
} from "@mui/material";
import { useEffect, useState } from "react";
import { Link, useNavigate } from "react-router-dom";
import type PermissionType from "../data/permission_type";
import permissions from "../services/permissions_service";

// State interfaces
interface FormState {
  employeeName: string;
  employeeSurname: string;
  typeId: string;
}

interface UIState {
  loading: boolean;
  typesLoading: boolean;
  error: string;
}

export default function PermissionForm() {
  const [formState, setFormState] = useState<FormState>({
    employeeName: "",
    employeeSurname: "",
    typeId: "",
  });

  const [uiState, setUIState] = useState<UIState>({
    loading: false,
    typesLoading: true,
    error: "",
  });

  const [permissionTypes, setPermissionTypes] = useState<PermissionType[]>([]);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchTypes = async () => {
      try {
        setUIState((prev) => ({ ...prev, typesLoading: true }));
        const permissionTypes = await permissions.getPermissionTypes();
        setPermissionTypes(permissionTypes);
      } catch (err) {
        console.error("Error fetching permission types:", err);
        setUIState((prev) => ({
          ...prev,
          error: "Failed to load permission types. Please try again later.",
        }));
      } finally {
        setUIState((prev) => ({ ...prev, typesLoading: false }));
      }
    };

    fetchTypes();
  }, []);

  const handleInputChange = (field: keyof FormState, value: string) => {
    setFormState((prev) => ({
      ...prev,
      [field]: value,
    }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    const { employeeName, employeeSurname, typeId } = formState;

    if (!employeeName.trim() || !employeeSurname.trim() || !typeId) {
      setUIState((prev) => ({
        ...prev,
        error: "Please fill all required fields.",
      }));
      return;
    }

    try {
      setUIState((prev) => ({
        ...prev,
        loading: true,
        error: "",
      }));

      await permissions.requestPermission({
        employeeName,
        employeeSurname,
        permissionTypeId: Number(typeId),
      });

      navigate("/");
    } catch (err) {
      console.error("Error creating permission:", err);
      setUIState((prev) => ({
        ...prev,
        error: "Failed to create permission. Please try again.",
        loading: false,
      }));
    }
  };

  const { employeeName, employeeSurname, typeId } = formState;
  const { loading, typesLoading, error } = uiState;

  return (
    <Container maxWidth="md">
      <Card elevation={3}>
        <CardContent sx={{ p: 3 }}>
          <Box sx={{ display: "flex", alignItems: "center", mb: 2 }}>
            <IconButton
              component={Link}
              to="/"
              sx={{ mr: 1 }}
              aria-label="back to list"
            >
              <ArrowBackIcon />
            </IconButton>
            <Typography variant="h5" component="h1" fontWeight="bold">
              Request New Permission
            </Typography>
          </Box>

          <Divider sx={{ mb: 3 }} />

          {error && (
            <Alert severity="error" sx={{ mb: 3 }}>
              {error}
            </Alert>
          )}

          {typesLoading ? (
            <Box sx={{ display: "flex", justifyContent: "center", p: 4 }}>
              <CircularProgress />
            </Box>
          ) : (
            <form onSubmit={handleSubmit}>
              <Grid container spacing={3}>
                <Grid size={4}>
                  <TextField
                    fullWidth
                    label="First Name"
                    value={employeeName}
                    onChange={(e) =>
                      handleInputChange("employeeName", e.target.value)
                    }
                    variant="outlined"
                    required
                    InputLabelProps={{ shrink: true }}
                    placeholder="Enter employee's first name"
                  />
                </Grid>
                <Grid size={4}>
                  <TextField
                    fullWidth
                    label="Last Name"
                    value={employeeSurname}
                    onChange={(e) =>
                      handleInputChange("employeeSurname", e.target.value)
                    }
                    variant="outlined"
                    required
                    InputLabelProps={{ shrink: true }}
                    placeholder="Enter employee's last name"
                  />
                </Grid>
                <Grid size={3}>
                  <TextField
                    fullWidth
                    select
                    label="Permission Type"
                    value={typeId}
                    onChange={(e) =>
                      handleInputChange("typeId", e.target.value)
                    }
                    variant="outlined"
                    required
                    InputLabelProps={{ shrink: true }}
                    placeholder="Select permission type"
                    disabled={permissionTypes.length === 0}
                  >
                    {permissionTypes.map((t) => (
                      <MenuItem key={t.id} value={t.id}>
                        {t.description}
                      </MenuItem>
                    ))}
                  </TextField>
                </Grid>

                <Grid sx={{ mt: 2 }}>
                  <Box
                    sx={{ display: "flex", justifyContent: "flex-end", gap: 2 }}
                  >
                    <Button
                      component={Link}
                      to="/"
                      variant="outlined"
                      color="inherit"
                    >
                      Cancel
                    </Button>
                    <Button
                      type="submit"
                      variant="contained"
                      color="primary"
                      disabled={loading}
                      startIcon={
                        loading ? <CircularProgress size={20} /> : <SaveIcon />
                      }
                    >
                      {loading ? "Submitting..." : "Submit Request"}
                    </Button>
                  </Box>
                </Grid>
              </Grid>
            </form>
          )}
        </CardContent>
      </Card>
    </Container>
  );
}
