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

export default function PermissionForm() {
  const [types, setTypes] = useState<PermissionType[]>([]);
  const [employeeName, setEmployeeName] = useState("");
  const [employeeSurname, setEmployeeSurname] = useState("");
  const [typeId, setTypeId] = useState("");
  const [loading, setLoading] = useState(false);
  const [typesLoading, setTypesLoading] = useState(true);
  const [error, setError] = useState("");
  const navigate = useNavigate();

  useEffect(() => {
    const fetchTypes = async () => {
      try {
        setTypesLoading(true);
        const permissionTypes = await permissions.getPermissionTypes();
        setTypes(permissionTypes);
      } catch (err) {
        console.error("Error fetching permission types:", err);
        setError("Failed to load permission types. Please try again later.");
      } finally {
        setTypesLoading(false);
      }
    };

    fetchTypes();
  }, []);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (!employeeName.trim() || !employeeSurname.trim() || !typeId) {
      setError("Please fill all required fields.");
      return;
    }

    try {
      setLoading(true);
      setError("");

      await permissions.requestPermission({
        employeeName,
        employeeSurname,
        permissionTypeId: Number(typeId),
      });

      navigate("/");
    } catch (err) {
      console.error("Error creating permission:", err);
      setError("Failed to create permission. Please try again.");
      setLoading(false);
    }
  };

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
                    onChange={(e) => setEmployeeName(e.target.value)}
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
                    onChange={(e) => setEmployeeSurname(e.target.value)}
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
                    onChange={(e) => setTypeId(e.target.value)}
                    variant="outlined"
                    required
                    InputLabelProps={{ shrink: true }}
                    placeholder="Select permission type"
                    disabled={types.length === 0}
                  >
                    {types.map((t) => (
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
